namespace SistemaPresatamos
{
    partial class FrmUsuario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNumeroEstudiante = new Label();
            txtNumeroEstudiante = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblCarrera = new Label();
            cmbCarrera = new ComboBox();
            lblRutaImagen = new Label();
            txtRutaImagen = new TextBox();
            lblEstado = new Label();
            chkEstadoUsuario = new CheckBox();
            pnlFormularioBase.SuspendLayout();
            SuspendLayout();
            // 
            // pnlFormularioBase
            // 
            pnlFormularioBase.Controls.Add(chkEstadoUsuario);
            pnlFormularioBase.Controls.Add(lblEstado);
            pnlFormularioBase.Controls.Add(txtRutaImagen);
            pnlFormularioBase.Controls.Add(lblRutaImagen);
            pnlFormularioBase.Controls.Add(cmbCarrera);
            pnlFormularioBase.Controls.Add(lblCarrera);
            pnlFormularioBase.Controls.Add(txtNombre);
            pnlFormularioBase.Controls.Add(lblNombre);
            pnlFormularioBase.Controls.Add(txtNumeroEstudiante);
            pnlFormularioBase.Controls.Add(lblNumeroEstudiante);
            pnlFormularioBase.Paint += pnlFormularioBase_Paint;
            pnlFormularioBase.Controls.SetChildIndex(txtId, 0);
            pnlFormularioBase.Controls.SetChildIndex(lblNumeroEstudiante, 0);
            pnlFormularioBase.Controls.SetChildIndex(txtNumeroEstudiante, 0);
            pnlFormularioBase.Controls.SetChildIndex(lblNombre, 0);
            pnlFormularioBase.Controls.SetChildIndex(txtNombre, 0);
            pnlFormularioBase.Controls.SetChildIndex(lblCarrera, 0);
            pnlFormularioBase.Controls.SetChildIndex(cmbCarrera, 0);
            pnlFormularioBase.Controls.SetChildIndex(lblRutaImagen, 0);
            pnlFormularioBase.Controls.SetChildIndex(txtRutaImagen, 0);
            pnlFormularioBase.Controls.SetChildIndex(lblEstado, 0);
            pnlFormularioBase.Controls.SetChildIndex(chkEstadoUsuario, 0);
            // 
            // lblNumeroEstudiante
            // 
            lblNumeroEstudiante.AutoSize = true;
            lblNumeroEstudiante.Location = new Point(179, 33);
            lblNumeroEstudiante.Name = "lblNumeroEstudiante";
            lblNumeroEstudiante.Size = new Size(128, 15);
            lblNumeroEstudiante.TabIndex = 2;
            lblNumeroEstudiante.Text = "Numero de Estudiante:";
            // 
            // txtNumeroEstudiante
            // 
            txtNumeroEstudiante.Location = new Point(313, 25);
            txtNumeroEstudiante.Name = "txtNumeroEstudiante";
            txtNumeroEstudiante.Size = new Size(100, 23);
            txtNumeroEstudiante.TabIndex = 3;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(253, 63);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(54, 15);
            lblNombre.TabIndex = 4;
            lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(313, 55);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(100, 23);
            txtNombre.TabIndex = 5;
            // 
            // lblCarrera
            // 
            lblCarrera.AutoSize = true;
            lblCarrera.Location = new Point(259, 92);
            lblCarrera.Name = "lblCarrera";
            lblCarrera.Size = new Size(48, 15);
            lblCarrera.TabIndex = 6;
            lblCarrera.Text = "Carrera:";
            // 
            // cmbCarrera
            // 
            cmbCarrera.FormattingEnabled = true;
            cmbCarrera.Items.AddRange(new object[] { "Ing. Ciencias Computacionales", "Ing. Energia renovables", "Ing. Nanotecnologia" });
            cmbCarrera.Location = new Point(313, 89);
            cmbCarrera.Name = "cmbCarrera";
            cmbCarrera.Size = new Size(121, 23);
            cmbCarrera.TabIndex = 7;
            // 
            // lblRutaImagen
            // 
            lblRutaImagen.AutoSize = true;
            lblRutaImagen.Location = new Point(233, 133);
            lblRutaImagen.Name = "lblRutaImagen";
            lblRutaImagen.Size = new Size(74, 15);
            lblRutaImagen.TabIndex = 8;
            lblRutaImagen.Text = "RutaImagen:";
            // 
            // txtRutaImagen
            // 
            txtRutaImagen.Location = new Point(313, 125);
            txtRutaImagen.Name = "txtRutaImagen";
            txtRutaImagen.Size = new Size(100, 23);
            txtRutaImagen.TabIndex = 9;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(262, 166);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(45, 15);
            lblEstado.TabIndex = 10;
            lblEstado.Text = "Estado:";
            // 
            // chkEstadoUsuario
            // 
            chkEstadoUsuario.AutoSize = true;
            chkEstadoUsuario.Location = new Point(313, 162);
            chkEstadoUsuario.Name = "chkEstadoUsuario";
            chkEstadoUsuario.Size = new Size(104, 19);
            chkEstadoUsuario.TabIndex = 11;
            chkEstadoUsuario.Text = "Estado Usuario";
            chkEstadoUsuario.UseVisualStyleBackColor = true;
            // 
            // FrmUsuario
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "FrmUsuario";
            Text = "FrmUsuario";
            Load += FrmUsuario_Load;
            pnlFormularioBase.ResumeLayout(false);
            pnlFormularioBase.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblCarrera;
        private TextBox txtNombre;
        private Label lblNombre;
        private TextBox txtNumeroEstudiante;
        private Label lblNumeroEstudiante;
        private CheckBox chkEstadoUsuario;
        private Label lblEstado;
        private TextBox txtRutaImagen;
        private Label lblRutaImagen;
        private ComboBox cmbCarrera;
    }
}