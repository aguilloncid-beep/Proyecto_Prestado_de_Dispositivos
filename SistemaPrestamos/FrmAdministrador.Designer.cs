namespace SistemaPresatamos
{
    partial class FrmAdministrador
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.GroupBox groupBoxAdministrador;
        private System.Windows.Forms.Label labelId;
        private System.Windows.Forms.Label labelCorreo;
        private System.Windows.Forms.Label labelNombre;
        private System.Windows.Forms.Label labelContrasena;
        private System.Windows.Forms.Label labelRutaImagen;
        private System.Windows.Forms.Label labelEstado;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.TextBox txtRutaImagen;
        private System.Windows.Forms.CheckBox chkEstadoAdmin;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.groupBoxAdministrador = new System.Windows.Forms.GroupBox();
            this.labelId = new System.Windows.Forms.Label();
            this.labelCorreo = new System.Windows.Forms.Label();
            this.labelNombre = new System.Windows.Forms.Label();
            this.labelContrasena = new System.Windows.Forms.Label();
            this.labelRutaImagen = new System.Windows.Forms.Label();
            this.labelEstado = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.txtRutaImagen = new System.Windows.Forms.TextBox();
            this.chkEstadoAdmin = new System.Windows.Forms.CheckBox();

            this.groupBoxAdministrador.SuspendLayout();
            this.SuspendLayout();

            // groupBoxAdministrador
            this.groupBoxAdministrador.Controls.Add(this.chkEstadoAdmin);
            this.groupBoxAdministrador.Controls.Add(this.txtRutaImagen);
            this.groupBoxAdministrador.Controls.Add(this.txtContrasena);
            this.groupBoxAdministrador.Controls.Add(this.txtNombre);
            this.groupBoxAdministrador.Controls.Add(this.txtCorreo);
            this.groupBoxAdministrador.Controls.Add(this.txtId);
            this.groupBoxAdministrador.Controls.Add(this.labelEstado);
            this.groupBoxAdministrador.Controls.Add(this.labelRutaImagen);
            this.groupBoxAdministrador.Controls.Add(this.labelContrasena);
            this.groupBoxAdministrador.Controls.Add(this.labelNombre);
            this.groupBoxAdministrador.Controls.Add(this.labelCorreo);
            this.groupBoxAdministrador.Controls.Add(this.labelId);
            this.groupBoxAdministrador.Location = new System.Drawing.Point(20, 20);
            this.groupBoxAdministrador.Name = "groupBoxAdministrador";
            this.groupBoxAdministrador.Size = new System.Drawing.Size(360, 250);
            this.groupBoxAdministrador.TabIndex = 0;
            this.groupBoxAdministrador.TabStop = false;
            this.groupBoxAdministrador.Text = "Administrador";

            // labelId
            this.labelId.AutoSize = true;
            this.labelId.Location = new System.Drawing.Point(20, 30);
            this.labelId.Name = "labelId";
            this.labelId.Size = new System.Drawing.Size(45, 15);
            this.labelId.Text = "IdAdmin";

            // txtId
            this.txtId.Location = new System.Drawing.Point(125, 27);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(200, 23);
            this.txtId.TabIndex = 0;

            // labelCorreo
            this.labelCorreo.AutoSize = true;
            this.labelCorreo.Location = new System.Drawing.Point(20, 62);
            this.labelCorreo.Name = "labelCorreo";
            this.labelCorreo.Size = new System.Drawing.Size(43, 15);
            this.labelCorreo.Text = "Correo";

            // txtCorreo
            this.txtCorreo.Location = new System.Drawing.Point(125, 59);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(200, 23);
            this.txtCorreo.TabIndex = 1;

            // labelNombre
            this.labelNombre.AutoSize = true;
            this.labelNombre.Location = new System.Drawing.Point(20, 94);
            this.labelNombre.Name = "labelNombre";
            this.labelNombre.Size = new System.Drawing.Size(51, 15);
            this.labelNombre.Text = "Nombre";

            // txtNombre
            this.txtNombre.Location = new System.Drawing.Point(125, 91);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(200, 23);
            this.txtNombre.TabIndex = 2;

            // labelContrasena
            this.labelContrasena.AutoSize = true;
            this.labelContrasena.Location = new System.Drawing.Point(20, 126);
            this.labelContrasena.Name = "labelContrasena";
            this.labelContrasena.Size = new System.Drawing.Size(69, 15);
            this.labelContrasena.Text = "Contraseña";

            // txtContrasena
            this.txtContrasena.Location = new System.Drawing.Point(125, 123);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.Size = new System.Drawing.Size(200, 23);
            this.txtContrasena.TabIndex = 3;
            this.txtContrasena.UseSystemPasswordChar = true;

            // labelRutaImagen
            this.labelRutaImagen.AutoSize = true;
            this.labelRutaImagen.Location = new System.Drawing.Point(20, 158);
            this.labelRutaImagen.Name = "labelRutaImagen";
            this.labelRutaImagen.Size = new System.Drawing.Size(74, 15);
            this.labelRutaImagen.Text = "Ruta Imagen";

            // txtRutaImagen
            this.txtRutaImagen.Location = new System.Drawing.Point(125, 155);
            this.txtRutaImagen.Name = "txtRutaImagen";
            this.txtRutaImagen.Size = new System.Drawing.Size(200, 23);
            this.txtRutaImagen.TabIndex = 4;

            // labelEstado
            this.labelEstado.AutoSize = true;
            this.labelEstado.Location = new System.Drawing.Point(20, 194);
            this.labelEstado.Name = "labelEstado";
            this.labelEstado.Size = new System.Drawing.Size(42, 15);
            this.labelEstado.Text = "Estado";

            // chkEstadoAdmin
            this.chkEstadoAdmin.AutoSize = true;
            this.chkEstadoAdmin.Location = new System.Drawing.Point(125, 192);
            this.chkEstadoAdmin.Name = "chkEstadoAdmin";
            this.chkEstadoAdmin.Size = new System.Drawing.Size(124, 19);
            this.chkEstadoAdmin.TabIndex = 5;
            this.chkEstadoAdmin.Text = "Disponible/No";
            this.chkEstadoAdmin.UseVisualStyleBackColor = true;

            // FrmAdministrador
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 300);
            this.Controls.Add(this.groupBoxAdministrador);
            this.Name = "FrmAdministrador";
            this.Text = "Administradores";
            this.Load += new System.EventHandler(this.FrmAdministrador_Load);

            this.groupBoxAdministrador.ResumeLayout(false);
            this.groupBoxAdministrador.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}