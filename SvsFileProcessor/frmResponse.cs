using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SvsFileProcessor {
	public partial class frmResponse : Form {
		public frmResponse(Action action) {
			InitializeComponent();
			Logger.LoggerWrite += Form1_onLoggerWrite;

			_action = action;
		}
		//---------------------------------------------------------------------------------------------------
		//-- Private
		//---------------------------------------------------------------------------------------------------
		private Action _action;


		private void Form1_onLoggerWrite(object sender, LoggerEventArgs e) {

			textBox1.Text += e.Message + "\r\n";
			Application.DoEvents();
		}
		//---------------------------------------------------------------------------------------------------
		private void frmResponse_Shown(object sender, EventArgs e) {

			Controller ctrlr = new Controller();
			ctrlr.Run(_action);

			Application.Exit();
		}
		//---------------------------------------------------------------------------------------------------
		private void textBox1_TextChanged(object sender, EventArgs e) {

			if (textBox1.Text.Length > 2024) {
				textBox1.Text = "";
			}

			if (textBox1.Text.Length > 0) {
				textBox1.SelectionStart = textBox1.Text.Length - 1;
				textBox1.ScrollToCaret();
				Application.DoEvents();
			}
		}

	}
}
