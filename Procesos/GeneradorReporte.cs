using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace SistemaPresatamos.Procesos
{
    public class ReporteLinea
    {
        public int IdPrestamo { get; set; }
        public string Alumno { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public int DiasPrestamo { get; set; }
        public int DiasRetraso { get; set; }
        public string Clasificacion { get; set; } = string.Empty;
    }

    public class ResumenAlumno
    {
        public string Alumno { get; set; } = string.Empty;
        public int Prestamos { get; set; }
        public int ConRetraso { get; set; }
        public int DiasRetrasoTotales { get; set; }
    }

    public class ReporteAvance
    {
        public ReporteAvance(int procesados, int total, ReporteLinea linea)
        {
            Procesados = procesados;
            Total = total;
            Linea = linea;
        }

        public int Procesados { get; private set; }
        public int Total { get; private set; }
        public ReporteLinea Linea { get; private set; }
        public int Porcentaje
        {
            get { return Total <= 0 ? 100 : Math.Min(100, (int)(100.0 * Procesados / Total)); }
        }
    }

    public class ReporteResultado
    {
        public List<ReporteLinea> Lineas { get; } = new List<ReporteLinea>();
        public List<ResumenAlumno> Resumen { get; } = new List<ResumenAlumno>();
    }

    public class ProgresoDirecto<T> : IProgress<T>
    {
        private readonly Action<T> _manejador;
        public ProgresoDirecto(Action<T> manejador) { _manejador = manejador; }
        public void Report(T valor) { _manejador(valor); }
    }

    public static class GeneradorReporte
    {
        public static ReporteResultado Generar(DataTable prestamos, int retardoPorRegistroMs,
                                               IProgress<ReporteAvance> progreso, CancellationToken token)
        {
            ReporteResultado resultado = new ReporteResultado();
            Dictionary<string, ResumenAlumno> porAlumno = new Dictionary<string, ResumenAlumno>();
            int total = prestamos.Rows.Count;
            int procesados = 0;

            foreach (DataRow fila in prestamos.Rows)
            {
                token.ThrowIfCancellationRequested();

                if (retardoPorRegistroMs > 0 && token.WaitHandle.WaitOne(retardoPorRegistroMs))
                    token.ThrowIfCancellationRequested();

                ReporteLinea linea = Analizar(fila);
                resultado.Lineas.Add(linea);

                ResumenAlumno? resumen;
                if (!porAlumno.TryGetValue(linea.Alumno, out resumen))
                {
                    resumen = new ResumenAlumno { Alumno = linea.Alumno };
                    porAlumno.Add(linea.Alumno, resumen);
                }
                resumen.Prestamos++;
                if (linea.DiasRetraso > 0) resumen.ConRetraso++;
                resumen.DiasRetrasoTotales += linea.DiasRetraso;

                procesados++;
                progreso.Report(new ReporteAvance(procesados, total, linea));
            }

            resultado.Resumen.AddRange(porAlumno.Values
                .OrderByDescending(r => r.DiasRetrasoTotales)
                .ThenBy(r => r.Alumno));
            return resultado;
        }

        private static ReporteLinea Analizar(DataRow fila)
        {
            DateTime prestamo = Convert.ToDateTime(fila["Prestamo"]);
            bool devuelto = fila["Entrega real"] != DBNull.Value;
            DateTime referencia = devuelto ? Convert.ToDateTime(fila["Entrega real"]) : DateTime.Today;
            int retraso = Convert.ToInt32(fila["Dias de retraso"]);
            string estado = Convert.ToString(fila["Estado"]) ?? string.Empty;

            string clasificacion;
            if (devuelto)
                clasificacion = retraso > 0 ? "Entregado con retraso" : "Entregado a tiempo";
            else
                clasificacion = (estado == "Mora" || retraso > 0) ? "Vencido (mora)" : "En curso";

            return new ReporteLinea
            {
                IdPrestamo = Convert.ToInt32(fila["Id"]),
                Alumno = Convert.ToString(fila["Alumno"]) ?? string.Empty,
                Material = Convert.ToString(fila["Material"]) ?? string.Empty,
                DiasPrestamo = Math.Max(0, (referencia.Date - prestamo.Date).Days),
                DiasRetraso = retraso,
                Clasificacion = clasificacion
            };
        }
    }
}
