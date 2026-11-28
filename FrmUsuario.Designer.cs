namespace SistemaPresatamos
{
    partial class FrmUsuario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

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
            components = new System.ComponentModel.Container();
            lblNumeroEstudiante = new Label();
            txtNumeroEstudiante = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblCorreo = new Label();
            txtCorreo = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            lblAyudaPassword = new Label();
            lblRol = new Label();
            cmbRol = new ComboBox();
            lblCarrera = new Label();
            cmbCarrera = new ComboBox();
            lblEstado = new Label();
            chkEstadoUsuario = new CheckBox();
            btnGuardar = new Button();
            btnActualizar = new Button();
            btnDesactivar = new Button();
            btnLimpiar = new Button();
            lblBuscar = new Label();
            txtBuscar = new TextBox();
            btnBuscar = new Button();
            gridUsuarios = new DataGridView();
            errorProvider1 = new ErrorProvider(components);
            statusStrip1 = new StatusStrip();
            lblEstadoBarra = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)gridUsuarios).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            pnlFormularioBase.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // pnlFormularioBase
            // 
            pnlFormularioBase.Controls.Add(gridUsuarios);
            pnlFormularioBase.Controls.Add(btnBuscar);
            pnlFormularioBase.Controls.Add(txtBuscar);
            pnlFormularioBase.Controls.Add(lblBuscar);
            pnlFormularioBase.Controls.Add(btnLimpiar);
            pnlFormularioBase.Controls.Add(btnDesactivar);
            pnlFormularioBase.Controls.Add(btnActualizar);
            pnlFormularioBase.Controls.Add(btnGuardar);
            pnlFormularioBase.Controls.Add(chkEstadoUsuario);
            pnlFormularioBase.Controls.Add(lblEstado);
            pnlFormularioBase.Controls.Add(cmbCarrera);
            pnlFormularioBase.Controls.Add(lblCarrera);
            pnlFormularioBase.Controls.Add(cmbRol);
            pnlFormularioBase.Controls.Add(lblRol);
            pnlFormularioBase.Controls.Add(lblAyudaPassword);
            pnlFormularioBase.Controls.Add(txtPassword);
            pnlFormularioBase.Controls.Add(lblPassword);
            pnlFormularioBase.Controls.Add(txtCorreo);
            pnlFormularioBase.Controls.Add(lblCorreo);
            pnlFormularioBase.Controls.Add(txtNombre);
            pnlFormularioBase.Controls.Add(lblNombre);
            pnlFormularioBase.Controls.Add(txtNumeroEstudiante);
            pnlFormularioBase.Controls.Add(lblNumeroEstudiante);
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
            txtNumeroEstudiante.Size = new Size(200, 23);
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
            txtNombre.Size = new Size(200, 23);
            txtNombre.TabIndex = 5;
            // 
            // lblCorreo
            // 
            lblCorreo.AutoSize = true;
            lblCorreo.Location = new Point(262, 93);
            lblCorreo.Name = "lblCorreo";
            lblCorreo.Size = new Size(45, 15);
            lblCorreo.TabIndex = 6;
            lblCorreo.Text = "Correo:";
            // 
            // txtCorreo
            // 
            txtCorreo.Location = new Point(313, 85);
            txtCorreo.Name = "txtCorreo";
            txtCorreo.Size = new Size(200, 23);
            txtCorreo.TabIndex = 7;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(238, 123);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(69, 15);
            lblPassword.TabIndex = 8;
            lblPassword.Text = "Contraseña:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(313, 115);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(200, 23);
            txtPassword.TabIndex = 9;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblAyudaPassword
            // 
            lblAyudaPassword.AutoSize = true;
            lblAyudaPassword.ForeColor = SystemColors.GrayText;
            lblAyudaPassword.Location = new Point(523, 123);
            lblAyudaPassword.Name = "lblAyudaPassword";
            lblAyudaPassword.Size = new Size(250, 15);
            lblAyudaPassword.TabIndex = 10;
            lblAyudaPassword.Text = "Al modificar, déjela vacía para conservar la actual";
            // 
            // lblRol
            // 
            lblRol.AutoSize = true;
            lblRol.Location = new Point(281, 153);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(26, 15);
            lblRol.TabIndex = 11;
            lblRol.Text = "Rol:";
            // 
            // cmbRol
            // 
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.FormattingEnabled = true;
            cmbRol.Items.AddRange(new object[] { "Alumno", "Administrador" });
            cmbRol.Location = new Point(313, 145);
            cmbRol.Name = "cmbRol";
            cmbRol.Size = new Size(200, 23);
            cmbRol.TabIndex = 12;
            // 
            // lblCarrera
            // 
            lblCarrera.AutoSize = true;
            lblCarrera.Location = new Point(259, 183);
            lblCarrera.Name = "lblCarrera";
            lblCarrera.Size = new Size(48, 15);
            lblCarrera.TabIndex = 13;
            lblCarrera.Text = "Carrera:";
            // 
            // cmbCarrera
            // 
            cmbCarrera.FormattingEnabled = true;
            cmbCarrera.Items.AddRange(new object[] { "Ing. Ciencias Computacionales", "Ing. Energia renovables", "Ing. Nanotecnologia" });
            cmbCarrera.Location = new Point(313, 175);
            cmbCarrera.Name = "cmbCarrera";
            cmbCarrera.Size = new Size(200, 23);
            cmbCarrera.TabIndex = 14;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(262, 212);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(45, 15);
            lblEstado.TabIndex = 15;
            lblEstado.Text = "Cuenta:";
            // 
            // chkEstadoUsuario
            // 
            chkEstadoUsuario.AutoSize = true;
            chkEstadoUsuario.Checked = true;
            chkEstadoUsuario.CheckState = CheckState.Checked;
            chkEstadoUsuario.Location = new Point(313, 208);
            chkEstadoUsuario.Name = "chkEstadoUsuario";
            chkEstadoUsuario.Size = new Size(60, 19);
            chkEstadoUsuario.TabIndex = 16;
            chkEstadoUsuario.Text = "Activa";
            chkEstadoUsuario.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(12, 245);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(110, 28);
            btnGuardar.TabIndex = 17;
            btnGuardar.Text = "Alta (nuevo)";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnActualizar
            // 
            btnActualizar.Location = new Point(132, 245);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(110, 28);
            btnActualizar.TabIndex = 18;
            btnActualizar.Text = "Modificar";
            btnActualizar.UseVisualStyleBackColor = true;
            btnActualizar.Click += btnActualizar_Click;
            // 
            // btnDesactivar
            // 
            btnDesactivar.Location = new Point(252, 245);
            btnDesactivar.Name = "btnDesactivar";
            btnDesactivar.Size = new Size(110, 28);
            btnDesactivar.TabIndex = 19;
            btnDesactivar.Text = "Desactivar";
            btnDesactivar.UseVisualStyleBackColor = true;
            btnDesactivar.Click += btnDesactivar_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Location = new Point(372, 245);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(110, 28);
            btnLimpiar.TabIndex = 20;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(12, 298);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(206, 15);
            lblBuscar.TabIndex = 21;
            lblBuscar.Text = "Buscar (correo, No. estudiante o Id):";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(225, 290);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(200, 23);
            txtBuscar.TabIndex = 22;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(435, 288);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(90, 28);
            btnBuscar.TabIndex = 23;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // gridUsuarios
            // 
            gridUsuarios.AllowUserToAddRows = false;
            gridUsuarios.AllowUserToDeleteRows = false;
            gridUsuarios.AllowUserToResizeRows = false;
            gridUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridUsuarios.BackgroundColor = SystemColors.Window;
            gridUsuarios.Location = new Point(12, 330);
            gridUsuarios.MultiSelect = false;
            gridUsuarios.Name = "gridUsuarios";
            gridUsuarios.ReadOnly = true;
            gridUsuarios.RowHeadersVisible = false;
            gridUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridUsuarios.Size = new Size(776, 235);
            gridUsuarios.TabIndex = 24;
            gridUsuarios.CellClick += gridUsuarios_CellClick;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblEstadoBarra });
            statusStrip1.Location = new Point(0, 578);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 1;
            // 
            // lblEstadoBarra
            // 
            lblEstadoBarra.Name = "lblEstadoBarra";
            lblEstadoBarra.Size = new Size(118, 17);
            lblEstadoBarra.Text = "Estado: listo";
            // 
            // FrmUsuario
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 634);
            Controls.Add(statusStrip1);
            Name = "FrmUsuario";
            Text = "Usuarios";
            Load += FrmUsuario_Load;
            ((System.ComponentModel.ISupportInitialize)gridUsuarios).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            pnlFormularioBase.ResumeLayout(false);
            pnlFormularioBase.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNumeroEstudiante;
        private TextBox txtNumeroEstudiante;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblCorreo;
        private TextBox txtCorreo;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblAyudaPassword;
        private Label lblRol;
        private ComboBox cmbRol;
        private Label lblCarrera;
        private ComboBox cmbCarrera;
        private Label lblEstado;
        private CheckBox chkEstadoUsuario;
        private Button btnGuardar;
        private Button btnActualizar;
        private Button btnDesactivar;
        private Button btnLimpiar;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private DataGridView gridUsuarios;
        private ErrorProvider errorProvider1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblEstadoBarra;
    }
}
