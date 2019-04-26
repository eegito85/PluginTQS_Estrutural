using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QINLOCO
{
    public partial class Quantitativo : Form
    {
        public Quantitativo()
        {
            InitializeComponent();
        }

        public string nomePavimento = "";
        public string caminhoPasta = "";
        ManipulacaoArquivos funcoes = new ManipulacaoArquivos();
        public List<string> listaInfoVigas = new List<string>();
        public List<string> listaInfoPilares = new List<string>();
        public List<string> listaInfoLajes = new List<string>();
        public double somaAreaEstruturadaV = 0;
        public double somaAreaFormaV = 0;
        public double somaVolumeConcretoV = 0;
        public double somaCompLinearV = 0;
        public double somaCompMedioV = 0;
        public double somaAreaEstruturadaP = 0;
        public double somaAreaFormaP = 0;
        public double somaVolumeConcretoP = 0;
        public double somaVolumeTopoP = 0;
        public double somaAreaEstruturadaL = 0;
        public double somaAreaFormaL = 0;
        public double somaVolumeConcretoL = 0;
        public double somaVolumeEPS = 0;
        public string alturaLajeN = "";
        public double volumeEPS = 0;
        public string dadosAtualizados = "";
        public string dadosMarcacoes = "";
        
        private void Quantitativo_Load(object sender, EventArgs e)
        {
            dadosMarcacoes = this.AccessibleName;
            nomePavimento = this.Text;
            caminhoPasta = this.Tag.ToString();
            limparTabela();
            //MessageBox.Show(caminhoPasta, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            
            listaInfoVigas = funcoes.lerQuantitativoViga(nomePavimento,caminhoPasta);
            listaInfoPilares = funcoes.lerQuantitativoPilares(nomePavimento,caminhoPasta);
            listaInfoLajes = funcoes.lerQuantitativoLajes(nomePavimento,caminhoPasta);
            //List<string> listaInfoLN = new List<string>();
            //listaInfoLN = funcoes.lerQuantitativoLajesNervuradas(nomePavimento);
            //foreach(string linha in listaInfoLN)
            //{
            //    MessageBox.Show(linha, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            carregaTabelaViga(listaInfoVigas);
            carregaTabelaPilares(listaInfoPilares);
            carregaTabelaLajes(listaInfoLajes);
            preencheCheckBox(dadosMarcacoes);
        }
        
        private void carregaTabelaViga(List<string> listaDadosVigas)
        {
            somaAreaEstruturadaV = 0;
            somaAreaFormaV = 0;
            somaVolumeConcretoV = 0;
            somaCompLinearV = 0;
            somaCompMedioV = 0;
            string[] dados = new string[6];
            double[] dadosN = new double[5];
            bool valor = true;
            int i = 0;
            int qtd = listaDadosVigas.Count;
        
            foreach (string dado in listaDadosVigas)
            {
                dado.TrimStart();
                dado.TrimEnd();
                char[] charSeparadores = new char[] { ' ' };
                dados = dado.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
        
                dadosN[0] = converterNumero(dados[1]);
                dadosN[1] = converterNumero(dados[2]);
                dadosN[2] = converterNumero(dados[3]);
                dadosN[3] = converterNumero(dados[4]);
                dadosN[4] = converterNumero(dados[5]);
        
                somaAreaEstruturadaV = somaAreaEstruturadaV + dadosN[0];
                somaAreaFormaV = somaAreaFormaV + dadosN[1];
                somaVolumeConcretoV = somaVolumeConcretoV + dadosN[2];
                somaCompLinearV = somaCompLinearV + dadosN[3];
                somaCompMedioV = somaCompMedioV + dadosN[4];
        
                dgvVigas.Rows.Add();
                dgvVigas.Rows[i].Cells[0].Value = valor;
                dgvVigas.Rows[i].Cells[1].Value = dados[0];
                dgvVigas.Rows[i].Cells[2].Value = dados[1];
                dgvVigas.Rows[i].Cells[3].Value = dados[2];
                dgvVigas.Rows[i].Cells[4].Value = dados[3];
                dgvVigas.Rows[i].Cells[5].Value = dados[4];
                dgvVigas.Rows[i].Cells[6].Value = dados[5];
                i++;
        
                //dgvVigas.Rows.Add(dados);
            }
        
            somaCompMedioV = Math.Round((somaCompMedioV / qtd), 2);
        
            txtTAreaFV.Text = Convert.ToString(somaAreaFormaV);
            txtTAreaV.Text = Convert.ToString(somaAreaEstruturadaV);
            txtTVolCV.Text = Convert.ToString(somaVolumeConcretoV);
            txtCompLV.Text = Convert.ToString(somaCompLinearV);
            txtCompMV.Text = Convert.ToString(somaCompMedioV);
        }
        
        private void carregaTabelaPilares(List<string> listaDadosPilares)
        {
            somaAreaEstruturadaP = 0;
            somaAreaFormaP = 0;
            somaVolumeConcretoP = 0;
            somaVolumeTopoP = 0;
            string[] dados = new string[5];
            double[] dadosN = new double[4];
            bool valor = true;
            int i = 0;
            foreach (string dado in listaDadosPilares)
            {
                dado.TrimStart();
                dado.TrimEnd();
                char[] charSeparadores = new char[] { ' ' };
                dados = dado.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
        
                dadosN[0] = converterNumero(dados[1]);
                dadosN[1] = converterNumero(dados[2]);
                dadosN[2] = converterNumero(dados[3]);
                dadosN[3] = converterNumero(dados[4]);
        
                somaAreaEstruturadaP = somaAreaEstruturadaP + dadosN[0];
                somaAreaFormaP = somaAreaFormaP + dadosN[1];
                somaVolumeConcretoP = somaVolumeConcretoP + dadosN[2];
                somaVolumeTopoP = somaVolumeTopoP + dadosN[3];
        
                dgvPilar.Rows.Add();
                dgvPilar.Rows[i].Cells[0].Value = valor;
                dgvPilar.Rows[i].Cells[1].Value = dados[0];
                dgvPilar.Rows[i].Cells[2].Value = dados[1];
                dgvPilar.Rows[i].Cells[3].Value = dados[2];
                dgvPilar.Rows[i].Cells[4].Value = dados[3];
                dgvPilar.Rows[i].Cells[5].Value = dados[4];
                i++;
                //dgvPilar.Rows.Add(dados);
            }
            txtAEsp.Text = Convert.ToString(somaAreaEstruturadaP);
            txtAfp.Text = Convert.ToString(somaAreaFormaP);
            txtVCp.Text = Convert.ToString(somaVolumeConcretoP);
            txtVTp.Text = Convert.ToString(somaVolumeTopoP);
        }
        
        private void carregaTabelaLajes(List<string> listaDadosLaje)
        {
            somaAreaEstruturadaL = 0;
            somaAreaFormaL = 0;
            somaVolumeConcretoL = 0;
            somaVolumeEPS = 0;
            volumeEPS = 0;
        
            double[] dadosN = new double[5];
            bool valor = true;
            int i = 0;
            foreach (string dado in listaDadosLaje)
            {
                dado.TrimStart();
                dado.TrimEnd();
                char[] charSeparadores = new char[] { ' ' };
                string[] dados = dado.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
        
                alturaLajeN = funcoes.retornaAlturaLajesNervuradas(nomePavimento, dados[0],caminhoPasta);
        
                dadosN[0] = converterNumero(dados[1]);
                dadosN[1] = converterNumero(dados[2]);
                dadosN[2] = converterNumero(dados[3]);
                dadosN[3] = converterNumero(alturaLajeN);
        
                if (alturaLajeN == "0.00"||alturaLajeN=="0"|| alturaLajeN == "" || dadosN[1] == 0) { volumeEPS = 0; }
                else { volumeEPS = funcoes.calculaVolumeEPS(dadosN[0], dadosN[3], dadosN[2]); }
        
                dadosN[4] = volumeEPS;
        
                somaAreaEstruturadaL = somaAreaEstruturadaL + dadosN[0];
                somaAreaFormaL = somaAreaFormaL + dadosN[1];
                somaVolumeConcretoL = somaVolumeConcretoL + dadosN[2];
                somaVolumeEPS = somaVolumeEPS + dadosN[4];
        
                dgvLajes.Rows.Add();
                dgvLajes.Rows[i].Cells[0].Value = valor;
                dgvLajes.Rows[i].Cells[1].Value = dados[0];
                dgvLajes.Rows[i].Cells[2].Value = dados[1];
                dgvLajes.Rows[i].Cells[3].Value = dados[2];
                dgvLajes.Rows[i].Cells[4].Value = dados[3];
                dgvLajes.Rows[i].Cells[5].Value = alturaLajeN;
                dgvLajes.Rows[i].Cells[6].Value = Convert.ToString(volumeEPS);
        
                i++;
        
            }
            txtAel.Text = Convert.ToString(somaAreaEstruturadaL);
            txtAfl.Text = Convert.ToString(somaAreaFormaL);
            txtVcl.Text = Convert.ToString(somaVolumeConcretoL);
            txtVolumeEPS.Text = Convert.ToString(somaVolumeEPS);
        }
        
        private void limparTabela()
        {
            dgvVigas.Rows.Clear();
            dgvPilar.Rows.Clear();
            dgvLajes.Rows.Clear();
        }
        
        private double converterNumero(string num)
        {
            num = num.TrimStart();
            num = num.TrimEnd();
            string[] numeros = new string[2];
            string p1 = "";
            string p2 = "";
            string numero = "";
            string ponto = ".";
            string virgula = ",";
            bool vPonto = num.Contains(ponto);
            bool vVirgula = num.Contains(virgula);
            //caso o número decimal tenha ponto
            if (vPonto == true && vVirgula == false)
            {
                numeros = num.Split('.');
                p1 = numeros[0];
                p2 = numeros[1];
                if (numeros[0] == "") { p1 = "0"; }
                numero = p1 + "," + p2;
            }
            else if (vPonto == false && vVirgula == true)
            {
                numero = num;
            }
            else if (vPonto == false && vVirgula == false)
            {
                numero = num + ",00";
            }
            else
            {
                MessageBox.Show("O número digitado está no formato incorreto", "Número Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numero = "0,00";
            }
        
            double f = Convert.ToDouble(numero);
            //string temp = num.Replace('.', ',');
            //double f = Convert.ToDouble(temp);
            //double f = Double.Parse(num);
            return f;
        }
        
        private void atualizaQuantitativoVigas()
        {
            somaAreaEstruturadaV = 0;
            somaAreaFormaV = 0;
            somaVolumeConcretoV = 0;
            somaCompLinearV = 0;
            somaCompMedioV = 0;
            string[] dados = new string[5];
            double[] dadosN = new double[5];
            int qtd = 0;
        
            foreach (DataGridViewRow row in dgvVigas.Rows)
            {
                if (Convert.ToString(row.Cells[1].Value) != "")
                {
                    //Verificar se está marcado
                    if ((bool)row.Cells[0].Value)
                    {
                        dados[0] = Convert.ToString(row.Cells[2].Value);
                        dados[1] = Convert.ToString(row.Cells[3].Value);
                        dados[2] = Convert.ToString(row.Cells[4].Value);
                        dados[3] = Convert.ToString(row.Cells[5].Value);
                        dados[4] = Convert.ToString(row.Cells[6].Value);
                        dadosN[0] = converterNumero(dados[0]);
                        dadosN[1] = converterNumero(dados[1]);
                        dadosN[2] = converterNumero(dados[2]);
                        dadosN[3] = converterNumero(dados[3]);
                        dadosN[4] = converterNumero(dados[4]);
                        somaAreaEstruturadaV = somaAreaEstruturadaV + dadosN[0];
                        somaAreaFormaV = somaAreaFormaV + dadosN[1];
                        somaVolumeConcretoV = somaVolumeConcretoV + dadosN[2];
                        somaCompLinearV = somaCompLinearV + dadosN[3];
                        somaCompMedioV = somaCompMedioV + dadosN[4];
                        qtd++;
                    }
        
                }
        
            }
            if (qtd == 0) { somaCompMedioV = 0; }
            else if (qtd > 0) { somaCompMedioV = Math.Round((somaCompMedioV / qtd), 2); }
        
            txtTAreaFV.Text = Convert.ToString(somaAreaFormaV);
            txtTAreaV.Text = Convert.ToString(somaAreaEstruturadaV);
            txtTVolCV.Text = Convert.ToString(somaVolumeConcretoV);
            txtCompLV.Text = Convert.ToString(somaCompLinearV);
            txtCompMV.Text = Convert.ToString(somaCompMedioV);
        }
        
        private void atualizaQuantitativoPilares()
        {
            somaAreaEstruturadaP = 0;
            somaAreaFormaP = 0;
            somaVolumeConcretoP = 0;
            somaVolumeTopoP = 0;
            string[] dados = new string[4];
            double[] dadosN = new double[4];
            int qtd = 0;
        
            foreach (DataGridViewRow row in dgvPilar.Rows)
            {
                if (Convert.ToString(row.Cells[1].Value) != "")
                {
                    //Verificar se está marcado
                    if ((bool)row.Cells[0].Value)
                    {
                        dados[0] = Convert.ToString(row.Cells[2].Value);
                        dados[1] = Convert.ToString(row.Cells[3].Value);
                        dados[2] = Convert.ToString(row.Cells[4].Value);
                        dados[3] = Convert.ToString(row.Cells[5].Value);
        
                        dadosN[0] = converterNumero(dados[0]);
                        dadosN[1] = converterNumero(dados[1]);
                        dadosN[2] = converterNumero(dados[2]);
                        dadosN[3] = converterNumero(dados[3]);
        
                        somaAreaEstruturadaP = somaAreaEstruturadaP + dadosN[0];
                        somaAreaFormaP = somaAreaFormaP + dadosN[1];
                        somaVolumeConcretoP = somaVolumeConcretoP + dadosN[2];
                        somaVolumeTopoP = somaVolumeTopoP + dadosN[3];
        
                        qtd++;
                    }
                }
            }
            txtAEsp.Text = Convert.ToString(somaAreaEstruturadaP);
            txtAfp.Text = Convert.ToString(somaAreaFormaP);
            txtVCp.Text = Convert.ToString(somaVolumeConcretoP);
            txtVTp.Text = Convert.ToString(somaVolumeTopoP);
        }
        
        private void atualizaQuantitativoLajes()
        {
            somaAreaEstruturadaL = 0;
            somaAreaFormaL = 0;
            somaVolumeConcretoL = 0;
            somaVolumeEPS = 0;
        
        
            string[] dados = new string[5];
            double[] dadosN = new double[5];
            int qtd = 0;
        
            foreach (DataGridViewRow row in dgvLajes.Rows)
            {
                if (Convert.ToString(row.Cells[1].Value) != "")
                {
                    //Verificar se está marcado
                    if ((bool)row.Cells[0].Value)
                    {
                        dados[0] = Convert.ToString(row.Cells[2].Value);
                        dados[1] = Convert.ToString(row.Cells[3].Value);
                        dados[2] = Convert.ToString(row.Cells[4].Value);
                        dados[3] = Convert.ToString(row.Cells[5].Value);
                        dados[4] = Convert.ToString(row.Cells[6].Value);
        
        
                        dadosN[0] = converterNumero(dados[0]);
                        dadosN[1] = converterNumero(dados[1]);
                        dadosN[2] = converterNumero(dados[2]);
                        dadosN[3] = converterNumero(dados[3]);
                        dadosN[4] = converterNumero(dados[4]);
        
                        if (dadosN[3] == 0) { dadosN[4] = 0; }
                        else { dadosN[4] = funcoes.calculaVolumeEPS(dadosN[0], dadosN[3], dadosN[2]); }
        
                        row.Cells[6].Value = Convert.ToString(dadosN[4]);
        
                        somaAreaEstruturadaL = somaAreaEstruturadaL + dadosN[0];
                        somaAreaFormaL = somaAreaFormaL + dadosN[1];
                        somaVolumeConcretoL = somaVolumeConcretoL + dadosN[2];
                        somaVolumeEPS = somaVolumeEPS + dadosN[4];
        
                        qtd++;
                    }
                }
            }
            txtAel.Text = Convert.ToString(somaAreaEstruturadaL);
            txtAfl.Text = Convert.ToString(somaAreaFormaL);
            txtVcl.Text = Convert.ToString(somaVolumeConcretoL);
            txtVolumeEPS.Text = Convert.ToString(somaVolumeEPS);
        }

        private void btAtualizar_Click(object sender, EventArgs e)
        {
            atualizaQuantitativoVigas();
            atualizaQuantitativoPilares();
            atualizaQuantitativoLajes();

            double sAreaEstruturadaV = funcoes.converteEmNumero(txtTAreaV.Text);
            double sAreaEstruturadaP = funcoes.converteEmNumero(txtAEsp.Text);
            double sAreaEstruturadaL = funcoes.converteEmNumero(txtAel.Text);

            string somaAreaEstruturada = Convert.ToString(sAreaEstruturadaV + sAreaEstruturadaP + sAreaEstruturadaL);

            dadosAtualizados = nomePavimento + ";" + txtTAreaFV.Text + ";" + txtAfp.Text + ";" + txtAfl.Text + ";" + txtTVolCV.Text + ";" + txtVCp.Text + ";" + txtVcl.Text + ";" + txtVolumeEPS.Text + ";" + somaAreaEstruturada;
            if (Application.OpenForms.OfType<Form1>().Count() > 0)
            {
                string name = Application.OpenForms.GetType().Name;
                
                foreach(Form frm in Application.OpenForms)
                {
                    name = frm.Name;
                    if(name=="Form1")
                    {
                        frm.AccessibleName = gravaSelecoesCheckBox(nomePavimento);
                        frm.Text = dadosAtualizados;
                        //MessageBox.Show(frm.AccessibleName, "Oi");
                    }
                }
            }
            this.Close();
        }

        private string gravaSelecoesCheckBox(string nomePavimento)
        {
            
            string cadeiaDigitos = "";

            foreach (DataGridViewRow row in dgvVigas.Rows)
            {
                if (Convert.ToString(row.Cells[1].Value) != "")
                {
                    //Verificar se está marcado
                    if ((bool)row.Cells[0].Value) { cadeiaDigitos = cadeiaDigitos + ";" + "1";  }
                    else { cadeiaDigitos = cadeiaDigitos + ";" + "0"; }
                }
            }
            foreach (DataGridViewRow row in dgvPilar.Rows)
            {
                if (Convert.ToString(row.Cells[1].Value) != "")
                {
                    //Verificar se está marcado
                    if ((bool)row.Cells[0].Value) { cadeiaDigitos = cadeiaDigitos + ";" + "1"; }
                    else { cadeiaDigitos = cadeiaDigitos + ";" + "0"; }
                }
            }
            foreach (DataGridViewRow row in dgvLajes.Rows)
            {
                if (Convert.ToString(row.Cells[1].Value) != "")
                {
                    //Verificar se está marcado
                    if ((bool)row.Cells[0].Value) { cadeiaDigitos = cadeiaDigitos + ";" + "1"; }
                    else { cadeiaDigitos = cadeiaDigitos + ";" + "0"; }
                }
            }
            cadeiaDigitos = nomePavimento + cadeiaDigitos;
            return cadeiaDigitos;
        }

        private void preencheCheckBox(string marcacoes)
        {
            if (marcacoes != "nada")
            {
                string[] elementos = marcacoes.Split(';');
                int i = 1;
                foreach (DataGridViewRow row in dgvVigas.Rows)
                {
                    if (Convert.ToString(row.Cells[1].Value) != "")
                    {
                        if (elementos[i] == "0")
                        { row.Cells[0].Value = false; }
                        else if (elementos[i] == "1")
                        { row.Cells[0].Value = true; }
                        i++;
                    }
                }
                foreach (DataGridViewRow row in dgvPilar.Rows)
                {
                    if (Convert.ToString(row.Cells[1].Value) != "")
                    {
                        if (elementos[i] == "0")
                        { row.Cells[0].Value = false; }
                        else if (elementos[i] == "1")
                        { row.Cells[0].Value = true; }
                        i++;
                    }
                }
                foreach (DataGridViewRow row in dgvLajes.Rows)
                {
                    if (Convert.ToString(row.Cells[1].Value) != "")
                    {
                        if (elementos[i] == "0")
                        { row.Cells[0].Value = false; }
                        else if (elementos[i] == "1")
                        { row.Cells[0].Value = true; }
                        i++;
                    }
                }


            }


        }
    }
}
