using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Procesos;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    public class FrmReportes : Form
    {
        private readonly NumericUpDown nudRetardo;
        private readonly Button btnBloqueante;
        private readonly Button btnGenerar;
        private readonly Button btnDetener;
        private readonly Button btnExportar;
        private readonly ProgressBar barra;
        private readonly Label lblPorcentaje;
        private readonly Label lblEstado;
        private readonly Label lblReloj;
        private readonly DataGridView gridDetalle;
        private readonly DataGridView gridResumen;
        private readonly System.Windows.Forms.Timer _reloj = new System.Windows.Forms.Timer();

        private CancellationTokenSource? _cts;
        private bool _cerrarAlTerminar;
        private ReporteResultado? _ultimoResultado;

        public FrmReportes()
        {
            Sesion.ExigirPersonal();

            Text = "Reporte de préstamos (procesamiento concurrente)";
            ClientSize = new Size(960, 580 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Simular carga por registro (ms):", 20, 18);
            nudRetardo = new NumericUpDown();
            nudRetardo.Location = new Point(215, 14);
            nudRetardo.Width = 70;
            nudRetardo.Minimum = 0;
            nudRetardo.Maximum = 3000;
            nudRetardo.Value = 150;
            Controls.Add(nudRetardo);

            btnBloqueante = Ui.Boton(this, "Generar (bloqueante, demostración)", 305, 12, 240, BtnBloqueante_Click);
            btnGenerar = Ui.Boton(this, "Generar en segundo plano", 555, 12, 190, BtnGenerar_Click);
            btnDetener = Ui.Boton(this, "Detener", 755, 12, 90, BtnDetener_Click);
            btnDetener.Enabled = false;

            barra = new ProgressBar();
            barra.Location = new Point(20, 52);
            barra.Size = new Size(820, 22);
            barra.Minimum = 0;
            barra.Maximum = 100;
            Controls.Add(barra);

            lblPorcentaje = Ui.Etiqueta(this, "0 %", 850, 56);
            lblEstado = Ui.Etiqueta(this, "Listo.", 20, 84);
            lblReloj = Ui.Etiqueta(this, string.Empty, 700, 84);
            Ui.Etiqueta(this, "Reloj de la interfaz (si se congela, el hilo de la UI está bloqueado):", 20, 106).ForeColor = SystemColors.GrayText;

            gridDetalle = Ui.Tabla(this, 20, 130, 920, 250);
            gridDetalle.Columns.Add("Id", "Id");
            gridDetalle.Columns.Add("Alumno", "Alumno");
            gridDetalle.Columns.Add("Material", "Material");
            gridDetalle.Columns.Add("DiasPrestamo", "Días de préstamo");
            gridDetalle.Columns.Add("DiasRetraso", "Días de retraso");
            gridDetalle.Columns.Add("Clasificacion", "Clasificación");

            Ui.Etiqueta(this, "Resumen por alumno (ordenado por días de retraso)", 20, 390);
            gridResumen = Ui.Tabla(this, 20, 410, 920, 120);
            gridResumen.Columns.Add("Alumno", "Alumno");
            gridResumen.Columns.Add("Prestamos", "Préstamos");
            gridResumen.Columns.Add("ConRetraso", "Con retraso");
            gridResumen.Columns.Add("DiasTotales", "Días de retraso totales");

            btnExportar = Ui.Boton(this, "Exportar CSV", 20, 540, 140, BtnExportar_Click);
            btnExportar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExportar.Enabled = false;
            Ui.Boton(this, "Cerrar", 830, 540, 110, (s, e) => Close()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            _reloj.Interval = 100;
            _reloj.Tick += (s, e) => lblReloj.Text = "Reloj UI: " + DateTime.Now.ToString("HH:mm:ss.f");
            _reloj.Start();

            FormClosing += FrmReportes_FormClosing;
            FormClosed += (s, e) => _reloj.Dispose();

            Autoria.Agregar(this);
        }

        private void BtnBloqueante_Click(object? sender, EventArgs e)
        {
            if (!Ui.Confirmar(this, "Esta demostración ejecuta el proceso en el hilo principal: la ventana se congelará hasta terminar. ¿Continuar?"))
                return;

            try
            {
                Preparar(true);
                lblEstado.Text = "Procesando en el hilo de la interfaz...";

                DataTable tabla = new PrestamoDAO().Consultar(null, null, null, null, false, null);
                ReporteResultado resultado = GeneradorReporte.Generar(
                    tabla, (int)nudRetardo.Value,
                    new ProgresoDirecto<ReporteAvance>(AplicarAvance),
                    CancellationToken.None);

                MostrarResultado(resultado);
                lblEstado.Text = "Terminado (versión bloqueante): " + resultado.Lineas.Count + " registros.";
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
            finally
            {
                Preparar(false);
            }
        }

        private async void BtnGenerar_Click(object? sender, EventArgs e)
        {
            if (_cts != null) return;

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            int retardo = (int)nudRetardo.Value;

            Preparar(true);
            EstablecerEstado("Iniciando proceso en segundo plano...");

            Progress<ReporteAvance> progreso = new Progress<ReporteAvance>(AplicarAvance);

            try
            {
                ReporteResultado resultado = await Task.Run(() =>
                {
                    EstablecerEstado("Consultando la base de datos (hilo " + Environment.CurrentManagedThreadId + ")...");
                    DataTable tabla = new PrestamoDAO().Consultar(null, null, null, null, false, null);

                    EstablecerEstado("Procesando " + tabla.Rows.Count + " registros...");
                    return GeneradorReporte.Generar(tabla, retardo, progreso, token);
                }, token);

                MostrarResultado(resultado);
                lblEstado.Text = "Reporte completo: " + resultado.Lineas.Count + " registros.";
            }
            catch (OperationCanceledException)
            {
                lblEstado.Text = "Proceso cancelado por el usuario.";
            }
            catch (Exception ex)
            {
                lblEstado.Text = "El proceso terminó con error.";
                Ui.Error(this, ex);
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                Preparar(false);

                if (_cerrarAlTerminar)
                    Close();
            }
        }

        private void BtnDetener_Click(object? sender, EventArgs e)
        {
            if (_cts == null) return;
            btnDetener.Enabled = false;
            EstablecerEstado("Cancelando...");
            _cts.Cancel();
        }

        private void AplicarAvance(ReporteAvance avance)
        {
            barra.Value = avance.Porcentaje;
            lblPorcentaje.Text = avance.Porcentaje + " %";

            ReporteLinea l = avance.Linea;
            gridDetalle.Rows.Add(l.IdPrestamo, l.Alumno, l.Material, l.DiasPrestamo, l.DiasRetraso, l.Clasificacion);
            gridDetalle.FirstDisplayedScrollingRowIndex = gridDetalle.Rows.Count - 1;

            lblEstado.Text = "Procesando " + avance.Procesados + " de " + avance.Total + "...";
        }

        private void EstablecerEstado(string texto)
        {
            if (lblEstado.InvokeRequired)
            {
                if (IsDisposed || !IsHandleCreated) return;
                lblEstado.BeginInvoke(new Action<string>(EstablecerEstado), texto);
                return;
            }

            lblEstado.Text = texto;
        }

        private void MostrarResultado(ReporteResultado resultado)
        {
            _ultimoResultado = resultado;
            gridResumen.Rows.Clear();
            foreach (ResumenAlumno r in resultado.Resumen)
                gridResumen.Rows.Add(r.Alumno, r.Prestamos, r.ConRetraso, r.DiasRetrasoTotales);

            barra.Value = 100;
            lblPorcentaje.Text = "100 %";
            btnExportar.Enabled = resultado.Lineas.Count > 0;
        }

        private void Preparar(bool ejecutando)
        {
            btnBloqueante.Enabled = !ejecutando;
            btnGenerar.Enabled = !ejecutando;
            nudRetardo.Enabled = !ejecutando;
            btnDetener.Enabled = ejecutando && _cts != null;

            if (ejecutando)
            {
                gridDetalle.Rows.Clear();
                gridResumen.Rows.Clear();
                barra.Value = 0;
                lblPorcentaje.Text = "0 %";
                btnExportar.Enabled = false;
                _ultimoResultado = null;
            }
        }

        private void FrmReportes_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_cts != null)
            {
                _cerrarAlTerminar = true;
                _cts.Cancel();
                e.Cancel = true;
            }
        }

        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            if (_ultimoResultado == null || _ultimoResultado.Lineas.Count == 0) return;

            using (SaveFileDialog dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "CSV (*.csv)|*.csv";
                dialogo.FileName = "reporte_prestamos_" + DateTime.Today.ToString("yyyyMMdd") + ".csv";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    List<string> lineas = new List<string> { "Id,Alumno,Material,DiasPrestamo,DiasRetraso,Clasificacion" };
                    foreach (ReporteLinea l in _ultimoResultado.Lineas)
                        lineas.Add(l.IdPrestamo + "," + Csv(l.Alumno) + "," + Csv(l.Material) + "," +
                                   l.DiasPrestamo + "," + l.DiasRetraso + "," + Csv(l.Clasificacion));

                    File.WriteAllLines(dialogo.FileName, lineas, new UTF8Encoding(true));
                    Ui.Aviso(this, "Reporte exportado correctamente.");
                }
                catch (Exception ex)
                {
                    Ui.Error(this, ex);
                }
            }
        }

        private static string Csv(string valor)
        {
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }
    }
}
