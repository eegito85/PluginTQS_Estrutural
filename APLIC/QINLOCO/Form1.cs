using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace QINLOCO
{
    public partial class Form1 : Form
    {
        TQS.Drawing2D.DrawingComponent dc;

        ManipulacaoArquivos funcoes = new ManipulacaoArquivos();
        CalculosMatematicos calculos = new CalculosMatematicos();
        ManipulacaoDwgDxf desenho = new ManipulacaoDwgDxf();
        Series Resistencia = new Series();

        public List<string> lstPavimentos = new List<string>();
        public string caminhoRaiz = "";
        public List<string> lstTodosDadosTabela = new List<string>();
        int quantidadePavimentos = 0;
        int controleForm = 0;
        double Fck = 0;
        double s = 0;
        public string[] dadosProjeto = new string[3];
        public double[,] dadosGrafico = new double[28, 2];
        public string[] relacaoTiposElementos = { "BLOCOS/SAPATAS", "CORTINAS/MUROS", "LAJE DA TAMPA", "PAREDES E LAJE DO FUNDO", "ARMADURA NEGATIVA", "ARMADURA POSITIVA", "ESCADAS E RAMPAS", "VIGAS DE BALDRAME", "DEMAIS VIGAS", "PILARES EM CONTATO COM O SOLO", "DEMAIS PILARES", "VIGAS", "LAJES" };
        public int indicaMudancaFckTabela = 0;
        public string pavimentoEditar = "";
        public int tipoMensagem = 0;
        public int indicaInicio = 0;
        public List<string> listaMarcacoes = new List<string>();
        public string caminhoArquivoDwg = "";
        string caminhoArquivoTmp = "";

        public Form1()
        {
            InitializeComponent();
            InitializeComponent2();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    textBox3.Text = (Convert.ToDouble(textBox1.Text) + Convert.ToDouble(textBox2.Text) + TQS.TQSTST.delta()).ToString();
        //    if (dc != null)
        //    {
        //        this.dc.IDrawable.AddLine(new TQS.Drawing2D.DrawingLine(new TQS.Geometry2D.Line(new TQS.Geometry2D.Point(0.0, 0.0), new TQS.Geometry2D.Point(100.0, 100.0))) { ColorMode = TQS.Drawing2D.ColorMode.Native });
        //        this.dc.IDrawable.AddLine(new TQS.Drawing2D.DrawingLine(new TQS.Geometry2D.Line(new TQS.Geometry2D.Point(100.0, 0.0), new TQS.Geometry2D.Point(0.0, 100.0))) { ColorMode = TQS.Drawing2D.ColorMode.Native });
        //        this.dc.ZoomExtents();
        //        pictureBox1.Refresh();
        //    }
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    Process cmd = new Process();
        //    cmd.StartInfo.FileName = "NULF.EXE";
        //    cmd.StartInfo.Arguments = "";
        //    cmd.StartInfo.CreateNoWindow = true;
        //    cmd.StartInfo.UseShellExecute = true;
        //    cmd.StartInfo.Verb = "runas";
        //    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        //    cmd.Start();
        //    cmd.WaitForExit();
        //
        //    InitializeComponent2();
        //}

        private void InitializeComponent2()
        {
            if (TQS.TQSTST.delta() != 0)
            {
                //button1.Enabled = false;
                pictureBox1.Controls.Clear();
                pictureBox1.BackColor = Color.Red;
                pictureBox1.Refresh();
                dc = null;
            }
            else
            {
                //button1.Enabled = true;
                this.dc = TQS.Drawing2D.Creation.DrawableComponentCreator.CreateDrawableComponent(TQS.Drawing2D.Creation.DrawableComponentTypes.TQSJan);
                this.dc.IDrawable.Clear();
                pictureBox1.Controls.Add((Control)this.dc);
                pictureBox1.Refresh();
            }
            preencheCabecalhoTabelas();
            desabilitaComandosDesenho();
        }

        //----->>>> FUNÇÃO PARA LISTAR TODOS OS PAVIMENTOS
        private void btCarregarPavimentos_Click(object sender, EventArgs e)
        {
            string caminho = null;
            resetar();
            try
            {
                FolderBrowserDialog caixaSelecaoPasta = new FolderBrowserDialog();
                caixaSelecaoPasta.RootFolder = Environment.SpecialFolder.MyComputer;
                caixaSelecaoPasta.ShowDialog();
                caminho = caixaSelecaoPasta.SelectedPath;
                //MessageBox.Show(caminho, "Oi");
                //OpenFileDialog caixaDialogo = new OpenFileDialog();
                //caixaDialogo.ShowDialog();
                //caminho = caixaDialogo.FileName;
                txtCaminhoPasta.Text = "";
                txtCaminhoPasta.Text = caminho;
                caminhoRaiz = funcoes.retornaCaminhoPasta(caminho);
                //lstPavimentos = funcoes.lerPavimentos(caminho);
                dadosProjeto = funcoes.lerDadosProjeto(caminhoRaiz);
                carregarInformacoesProjeto();
                lstPavimentos = funcoes.lerPavimentos(caminhoRaiz);
                lstPavimentos = inverteOrdemLista(lstPavimentos);
                carregarListBoxPavimentos(lstPavimentos);
                carregarTabela(lstPavimentos, caminhoRaiz);
                //cbTipoAgregado.SelectedIndex = 1;
                //indicaFck = 0;
                //cbClasseAgressividade.SelectedIndex = carregarCAA() - 1;
                cbS.SelectedIndex = 1;
                if (indicaInicio == 0)
                {
                    this.graficoResistencia.Titles.Add("Resistências do concreto (MPa)");
                    indicaInicio = 1;

                }
                carregaTodosCobrimentos();
                habilitaComandosDesenho();
                abrirDesenhoDxf();
                try { File.Delete("C:\\TQSW\\SUPORTE\\DP\\DPS\\QINLOCO\\RESUMO_MATERIAL_TEMP.DXF"); }
                catch { }
                criarArquivoTemporario();

            }
            catch (Exception ex) { MessageBox.Show("O usuário não selecionou nenhuma pasta", "Projeto não carregado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int carregarCAA()
        {
            TQS.AcessoN.Edificio edif = new TQS.AcessoN.Edificio();
            string str = edif.diretorios.Espacial;
            int CAA = edif.gerais.Classe_Agressividade_Ambiental;
            return CAA;
        }

        public void carregarTabela(List<string> lista, string caminhoPasta)
        {
            List<string> lstNovo = new List<string>();
            string[] info = new string[9];
            string[] qtds = new string[8];
            quantidadePavimentos = lista.Count;

            foreach (string pav in lista)
            {
                string[] dados = funcoes.lerDadosPavimento(pav, caminhoPasta);
                lstNovo = funcoes.retornaListaSomatorios(pav, caminhoPasta);
                info[0] = pav;
                info[1] = dados[1];
                info[2] = dados[2];
                info[3] = dados[3];
                info[4] = dados[4];
                info[5] = dados[5];
                info[6] = lstNovo[0];
                info[7] = lstNovo[1];
                info[8] = lstNovo[2];
                dgvGerais.Rows.Add(info);
                qtds[0] = info[0];
                qtds[1] = lstNovo[3];
                qtds[2] = lstNovo[4];
                qtds[3] = lstNovo[5];
                qtds[4] = lstNovo[6];
                qtds[5] = lstNovo[7];
                qtds[6] = lstNovo[8];
                qtds[7] = retornaSomaVolumeEPS(pav, caminhoPasta);
                dgvQuantitativo.Rows.Add(qtds);
                lstTodosDadosTabela.Add(info[0]);
                lstTodosDadosTabela.Add(info[1]);
                lstTodosDadosTabela.Add(info[2]);
                lstTodosDadosTabela.Add(info[3]);
                lstTodosDadosTabela.Add(info[4]);
                lstTodosDadosTabela.Add(info[5]);
                lstTodosDadosTabela.Add(info[6]);
                lstTodosDadosTabela.Add(info[7]);
                lstTodosDadosTabela.Add(info[8]);
                lstTodosDadosTabela.Add(qtds[1]);
                lstTodosDadosTabela.Add(qtds[2]);
                lstTodosDadosTabela.Add(qtds[3]);
                lstTodosDadosTabela.Add(qtds[4]);
                lstTodosDadosTabela.Add(qtds[5]);
                lstTodosDadosTabela.Add(qtds[6]);
                lstTodosDadosTabela.Add(qtds[7]);

            }
        }

        public void carregarTabelaResumo()
        {
            
            double somaAfV = 0;
            double somaAfP = 0;
            double somaAfL = 0;

            double somaVcV = 0;
            double somaVcP = 0;
            double somaVcL = 0;

            limparTabelaResumo();

            int qtd = lstTodosDadosTabela.Count();
            List<string> pavimentosSelecionados = new List<string>();
            try
            {
                foreach (string nomePavimento in lbPavimentosSelecionados.Items)
                {
                    if (nomePavimento != null || nomePavimento != "")
                    {
                        pavimentosSelecionados.Add(nomePavimento);
                    }
                }

                int numeroPavSelecionados = pavimentosSelecionados.Count;
                for (int i = 0; i < numeroPavSelecionados; i++)
                {
                    for (int j = 0; j < qtd; j = j + 16)
                    {
                        if (pavimentosSelecionados[i] == lstTodosDadosTabela[j])
                        {
                            somaAfV = somaAfV + funcoes.converteEmNumero(lstTodosDadosTabela[j + 9]);
                            somaAfP = somaAfP + funcoes.converteEmNumero(lstTodosDadosTabela[j + 10]);
                            somaAfL = somaAfL + funcoes.converteEmNumero(lstTodosDadosTabela[j + 11]);

                            somaVcV = somaVcV + funcoes.converteEmNumero(lstTodosDadosTabela[j + 12]);
                            somaVcP = somaVcP + funcoes.converteEmNumero(lstTodosDadosTabela[j + 13]);
                            somaVcL = somaVcL + funcoes.converteEmNumero(lstTodosDadosTabela[j + 14]);

                            //MessageBox.Show(pavimentosSelecionados[i], "Pavimento Selecionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                txtVf.Text = Convert.ToString(somaAfV);
                txtPf.Text = Convert.ToString(somaAfP);
                txtLf.Text = Convert.ToString(somaAfL);
                txtVc.Text = Convert.ToString(somaVcV);
                txtPc.Text = Convert.ToString(somaVcP);
                txtLc.Text = Convert.ToString(somaVcL);
                //dgvTabelaResumo.Rows.Add(Convert.ToString(somaAfV), Convert.ToString(somaAfP), Convert.ToString(somaAfL));
                //dgvTabelaResumo.Rows.Add(Convert.ToString(somaVcV), Convert.ToString(somaVcP), Convert.ToString(somaVcL));
            }
            catch { }
            

        }

        public void carregarListBoxPavimentos(List<string> lista)
        {
            foreach (string pav in lista)
            {
                lbPavimentos.Items.Add(pav);
            }
        }

        private void btAdicionar_Click(object sender, EventArgs e)
        {
            string nomeSelecionado = "";
            int indicadorRepetido = 0;
            try
            {
                nomeSelecionado = lbPavimentos.SelectedItem.ToString();
                foreach(string pavimento in lbPavimentosSelecionados.Items)
                {
                    if(nomeSelecionado==pavimento) { indicadorRepetido = 1; }
                }
                if (indicadorRepetido == 0) { lbPavimentosSelecionados.Items.Add(nomeSelecionado); }
                carregarTabelaResumo();
                organizarListBoxPavimentos();
            }
            catch { }
        }

        private void btRemoverItem_Click(object sender, EventArgs e)
        {
            string nomeSelecionado = "";
            try
            {
                nomeSelecionado = lbPavimentosSelecionados.SelectedItem.ToString();
                lbPavimentosSelecionados.Items.Remove(nomeSelecionado);
                carregarTabelaResumo();
                //organizarListBoxPavimentos();
            }
            catch { }
        }

        private void btRemoveTudo_Click(object sender, EventArgs e)
        {
            lbPavimentosSelecionados.Items.Clear();
            limparTabelaResumo();
        }

        //private void pictureBox2_MouseEnter(object sender, EventArgs e)
        //{
        //    ToolTip mensagem = new ToolTip();
        //    mensagem.IsBalloon = true;
        //    mensagem.ToolTipIcon = ToolTipIcon.Info;
        //    mensagem.Show("Selecione o arquivo CARDWG.LST", pbInfo);
        //}

        private void limparTabelaResumo()
        {
            //dgvTabelaResumo.Rows.Clear();
        }

        private void dgvQuantitativo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Quantitativo frmQtd = new Quantitativo();
            try
            {
                if (e.ColumnIndex == 0)
                {
                    string nomepavimento = lstPavimentos[e.RowIndex];
                    string marcacoesChckBox = carregaListaMarcacoes(nomepavimento);
                    frmQtd.AccessibleName = marcacoesChckBox;
                    frmQtd.Tag = caminhoRaiz;
                    frmQtd.Text = nomepavimento;
                    frmQtd.ShowDialog();
                }
            }
            catch { }
        }

        private void atualizaTabelas(string dadosAtualizados)
        {
            //MessageBox.Show(dadosAtualizados, "Oi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            controleForm = 1;
            this.Text = "QINLOCO";
            string[] novasInformacoes = dadosAtualizados.Split(';');
            string pavimentoAlterado = novasInformacoes[0];
            int posicaoPavimentoAlterado = 0;
            foreach(string info in lstTodosDadosTabela)
            {
                if(info==pavimentoAlterado) { posicaoPavimentoAlterado = lstTodosDadosTabela.IndexOf(pavimentoAlterado); }
            }
            lstTodosDadosTabela[posicaoPavimentoAlterado + 6] = novasInformacoes[8];
            lstTodosDadosTabela[posicaoPavimentoAlterado + 9] = novasInformacoes[1];
            lstTodosDadosTabela[posicaoPavimentoAlterado + 10] = novasInformacoes[2];
            lstTodosDadosTabela[posicaoPavimentoAlterado + 11] = novasInformacoes[3];
            lstTodosDadosTabela[posicaoPavimentoAlterado + 12] = novasInformacoes[4];
            lstTodosDadosTabela[posicaoPavimentoAlterado + 13] = novasInformacoes[5];
            lstTodosDadosTabela[posicaoPavimentoAlterado + 14] = novasInformacoes[6];
            lstTodosDadosTabela[posicaoPavimentoAlterado + 15] = novasInformacoes[7];

            double Afv = funcoes.converteEmNumero(novasInformacoes[1]);
            double Afp = funcoes.converteEmNumero(novasInformacoes[2]);
            double Afl = funcoes.converteEmNumero(novasInformacoes[3]);
            double Vcv = funcoes.converteEmNumero(novasInformacoes[4]);
            double Vcp = funcoes.converteEmNumero(novasInformacoes[5]);
            double Vcl = funcoes.converteEmNumero(novasInformacoes[6]);
            double Vn = funcoes.converteEmNumero(novasInformacoes[7]);

            string somaA = Convert.ToString(Afv + Afp + Afl);
            string somaV = Convert.ToString(Vcv + Vcp + Vcl);

            lstTodosDadosTabela[posicaoPavimentoAlterado + 7] = somaA;
            lstTodosDadosTabela[posicaoPavimentoAlterado + 8] = somaV;

            dgvGerais.Rows[posicaoPavimentoAlterado / 16].Cells[6].Value = novasInformacoes[8];
            dgvGerais.Rows[posicaoPavimentoAlterado / 16].Cells[7].Value = somaA;
            dgvGerais.Rows[posicaoPavimentoAlterado / 16].Cells[8].Value = somaV;
            dgvQuantitativo.Rows[posicaoPavimentoAlterado / 16].Cells[1].Value = novasInformacoes[1];
            dgvQuantitativo.Rows[posicaoPavimentoAlterado / 16].Cells[2].Value = novasInformacoes[2];
            dgvQuantitativo.Rows[posicaoPavimentoAlterado / 16].Cells[3].Value = novasInformacoes[3];
            dgvQuantitativo.Rows[posicaoPavimentoAlterado / 16].Cells[4].Value = novasInformacoes[4];
            dgvQuantitativo.Rows[posicaoPavimentoAlterado / 16].Cells[5].Value = novasInformacoes[5];
            dgvQuantitativo.Rows[posicaoPavimentoAlterado / 16].Cells[6].Value = novasInformacoes[6];
            dgvQuantitativo.Rows[posicaoPavimentoAlterado / 16].Cells[7].Value = novasInformacoes[7];
            controleForm = 0;

            carregarTabelaResumo();
        }

        private string retornaSomaVolumeEPS(string pavimento,string caminhoPasta)
        {
            List<string> listaDadosLaje = funcoes.lerQuantitativoLajes(pavimento, caminhoPasta);
            double somaVolumeEPS = 0;
            double vEPS = 0;
            double alturaLajeN = 0;

            double[] dadosN = new double[5];
            
            int i = 0;
            foreach (string dado in listaDadosLaje)
            {
                dado.TrimStart();
                dado.TrimEnd();
                char[] charSeparadores = new char[] { ' ' };
                string[] dados = dado.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);

                alturaLajeN = funcoes.converteEmNumero(funcoes.retornaAlturaLajesNervuradas(pavimento, dados[0], caminhoPasta));

                dadosN[0] = funcoes.converteEmNumero(dados[1]);
                dadosN[1] = funcoes.converteEmNumero(dados[2]);
                dadosN[2] = funcoes.converteEmNumero(dados[3]);

                if (alturaLajeN == 0 || dadosN[1] == 0) { vEPS = 0; }
                else { vEPS = funcoes.calculaVolumeEPS(dadosN[0], alturaLajeN, dadosN[2]); }
                somaVolumeEPS = somaVolumeEPS + vEPS;

                i++;
            }
            return (Convert.ToString(somaVolumeEPS));
        }

        private void Form1_TextChanged(object sender, EventArgs e)
        {
            if(controleForm==0)
            {
                string novosDados = this.Text;
                //labelTeste.Text = this.AccessibleName;
                //MessageBox.Show(labelTeste.Text, "Olá de novo");
                atualizaTabelas(novosDados);
                salvaListaMarcacoes(this.AccessibleName);
            }   
        }

        private void salvaListaMarcacoes(string dadosMarcacoes)
        {
            string[] dados = dadosMarcacoes.Split(';');
            int indicador = 0;
            foreach(string elemento in listaMarcacoes)
            {
                string[] elementos = elemento.Split(';');
                if (dados[0] == elementos[0])
                {
                    indicador = 1; // o pavimento já esiste
                    listaMarcacoes.Remove(elemento);
                    listaMarcacoes.Add(dadosMarcacoes);
                } 
            }
            if (indicador == 0) { listaMarcacoes.Add(dadosMarcacoes); }
        }

        private string carregaListaMarcacoes(string nPavimento)
        {
            string dado = "nada";
            foreach (string elemento in listaMarcacoes)
            {
                string[] elementos = elemento.Split(';');
                if (elementos[0]==nPavimento) { dado = elemento; } 
            }
            return dado;
        }

        private double alfaE(string tipoAgregado)
        {
            double alfaE = 0;
            switch (tipoAgregado)
            {
                case "Basalto":
                    alfaE = 1.2;
                    break;
                case "Diabásio":
                    alfaE = 1.2;
                    break;
                case "Granito":
                    alfaE = 1;
                    break;
                case "Gnaisse":
                    alfaE = 1;
                    break;
                case "Calcário":
                    alfaE = 0.9;
                    break;
                case "Arenito":
                    alfaE = 0.7;
                    break;
            }
            
            return alfaE;
        }

        private void preencheTabelaResistencia(double s,double fck)
        {
            //dgvResistencia.Rows.Clear();
            double resistencia = 0;
            double eci = 0;
            double ecs = 0;
            double aE = 0;
            string tipoAgregado = cbTipoAgregado.Text;
            //MessageBox.Show(tipoAgregado, "Olá");

            aE = alfaE(tipoAgregado);

            for (int i = 1; i < 29; i++)
            {
                resistencia = calculos.calculaResistencia(fck, s, i);
                eci = calculos.calculaEci(fck, aE, s, i);
                ecs = calculos.calculaEcs(fck, aE, s, i);
                //if (i == 7 || i == 14 || i == 21 || i == 28)
                //{
                //    dgvResistencia.Rows.Add(Convert.ToString(i), Convert.ToString(resistencia), Convert.ToString(eci), Convert.ToString(ecs));
                //}
                if (i == 7)
                {
                    txtR7.Text = Convert.ToString(resistencia);
                    txtEci7.Text = Convert.ToString(eci);
                }
                else if (i == 14)
                {
                    txtR14.Text = Convert.ToString(resistencia);
                    txtEci14.Text = Convert.ToString(eci);
                }
                else if (i == 21)
                {
                    txtR21.Text = Convert.ToString(resistencia);
                    txtEci21.Text = Convert.ToString(eci);
                }
                else if (i == 28)
                {
                    txtR28.Text = Convert.ToString(resistencia);
                    txtEci28.Text = Convert.ToString(eci);
                }
                dadosGrafico[i - 1, 0] = i;
                dadosGrafico[i - 1, 1] = resistencia;
            }

        }

        private void lbS_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Fck = Convert.ToDouble(lbfck.Text);
                s = Convert.ToDouble(lbS.Text);
                s = s / 100;
                preencheTabelaResistencia(s, Fck);
                plotaGrafico(dadosGrafico);
            }
            catch { }
        }

        private void cbS_SelectedIndexChanged(object sender, EventArgs e)
        {
            string itemSelecionado = cbS.SelectedItem.ToString();
            //MessageBox.Show(itemSelecionado, "Olá");
            string valorS = "";

            switch (itemSelecionado)
            {
                case "CP III - CP IV":
                    valorS = "0.38";
                    break;
                case "CP I - CP II":
                    valorS = "0.25";
                    break;
                case "CP V e AR I":
                    valorS = "0.20";
                    break;
            }
            lbS.Text = valorS;
        }

        private void plotaGrafico(double[,] dados)
        {
            //double[] dias = new double[28];
            //double[] resistencias = new double[28];
            string[] dias = new string[28];
            double[] resistencias = new double[28];
            
            this.graficoResistencia.Series.Clear();

            for (int i = 0; i < 28; i++)
            {
                dias[i] = Convert.ToString(dados[i, 0]);
                resistencias[i] = dados[i, 1];
            }

            //this.graficoResistencia.Titles.Add("Resistências do concreto (MPa)");
            Resistencia.ChartType = SeriesChartType.Line;
            //this.graficoResistencia.ChartAreas[0].AxisX.StripLines.Clear();
            this.graficoResistencia.ChartAreas[0].AxisX.Interval = 7;
            this.graficoResistencia.ChartAreas[0].AxisX.Maximum = 30;
            this.graficoResistencia.ChartAreas[0].AxisX.Minimum = 0;
            Resistencia.Points.DataBindXY(dias, resistencias);
            this.graficoResistencia.Series.Add(Resistencia);

        }

        //----->>>> FUNÇÃO A SER CARREGADA SOMENTE NO INÍCIO
        private void carregaTodosCobrimentos()
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);
                double cobrimento = 0;

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt2.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[0], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt1.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt4.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[1], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt3.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt6.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[2], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt5.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt8.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[3], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt7.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt10.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[4], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt9.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt12.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[5], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt11.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt14.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[6], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt13.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt16.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[7], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt15.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt18.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[8], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt17.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt20.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[9], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt19.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt22.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[10], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCA(valorfck, classeAgressividade);
                txt21.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt24.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[11], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCP(valorfck, classeAgressividade);
                txt23.Text = Convert.ToString(cobrimento);

                cobrimentoAdotado = txt26.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                cobrimento = calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[12], cobrimentoAdotado, classeAgressividade, valorfck);
                cobrimento = cobrimento - calculos.fatorCorrecaoCobrimentoCP(valorfck, classeAgressividade);
                txt25.Text = Convert.ToString(cobrimento);
            }
            catch { }
            //indicaFck = 1;
            
        }

        private void txt2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt2.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt1.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[0], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt4.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt3.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[1], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }


                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt6.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt5.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[2], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt8.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt7.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[3], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt10.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt9.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[4], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt12_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt12.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt11.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[5], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt14_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt14.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt13.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[6], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt16_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }


                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt16.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt15.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[7], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt18_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt18.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt17.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[8], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt20_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt20.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt19.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[9], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt22_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt22.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt21.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[10], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt24_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt24.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt22.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[11], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void txt26_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string classeAgressividade = cbClasseAgressividade.Text;
                double valorfck = Convert.ToDouble(lbfck.Text);

                int indicadorControleQualidade = 0;
                if (cbControleQualidade.Checked == true) { indicadorControleQualidade = 1; }
                else if (cbControleQualidade.Checked == false) { indicadorControleQualidade = 0; }

                string cobrimentoAdotado = "";

                cobrimentoAdotado = txt26.Text;
                if (cobrimentoAdotado == null || cobrimentoAdotado == "") { cobrimentoAdotado = "0"; }
                txt25.Text = Convert.ToString(calculos.retornaCobrimentoTotal(indicadorControleQualidade, relacaoTiposElementos[12], cobrimentoAdotado, classeAgressividade, valorfck));
            }
            catch { }
        }

        private void cbClasseAgressividade_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                double vFck = funcoes.converteEmNumero(lbfck.Text);
                string caa = cbClasseAgressividade.Text;
                string aviso = calculos.mensagemAlertaFck(vFck, caa);
                carregaTodosCobrimentos();
            }
            catch { }
        }

        private void cbControleQualidade_CheckedChanged(object sender, EventArgs e)
        {
            carregaTodosCobrimentos();
        }

        private void cbProtendido_CheckedChanged(object sender, EventArgs e)
        {
            carregaTodosCobrimentos();
        }

        private void cbTipoAgregado_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipoAgregado = cbTipoAgregado.Text;
            double alfaE = 0;
            switch (tipoAgregado)
            {
                case "Basalto":
                    alfaE = 1.2;
                    break;
                case "Granito":
                    alfaE = 1;
                    break;
                case "Calcário":
                    alfaE = 0.9;
                    break;
                case "Arenito":
                    alfaE = 0.7;
                    break;
            }
            try
            {
                s = Convert.ToDouble(lbS.Text);
                s = s / 100;
                double fck = Convert.ToDouble(lbfck.Text);
                lbEci.Text = Convert.ToString(calculos.calculaDEci(fck, alfaE));
                lbEcs.Text = Convert.ToString(calculos.calculaDEcs(fck, alfaE));
                preencheTabelaResistencia(s, Fck);
                plotaGrafico(dadosGrafico);
            }
            catch { }
            
        }

        private List<string> inverteOrdemLista(List<string> lista)
        {
            List<string> listaInversa = new List<string>();
            string elemento = "";
            int n = lista.Count();
            for(int i=(n-1);i>=0;i--)
            {
                elemento = lista[i];
                listaInversa.Add(elemento);
            }
            return listaInversa;
        }

        private void lbPavimentosSelecionados_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                tipoMensagem = 2;
                aparecerMensagem(tipoMensagem);
                string pavSelecionado = lbPavimentosSelecionados.SelectedItem.ToString();
                //MessageBox.Show(pavSelecionado, "Olá Doido!");
                int num = lstTodosDadosTabela.Count();
                string fckSelecionado = "";
                double fck = 0;
                double aE = 0;
                string tipoAgregado = cbTipoAgregado.Text;
                s = funcoes.converteEmNumero(lbS.Text);

                aE = alfaE(tipoAgregado);

                for (int i = 0; i < num; i = i + 16)
                {
                    if (pavSelecionado == lstTodosDadosTabela[i])
                    {
                        fckSelecionado = lstTodosDadosTabela[i + 1];
                        fck = funcoes.converteEmNumero(fckSelecionado);
                    }
                }

                //lbPavimento.Visible = true;
                lbPavimento.Text = pavSelecionado;
                gbPavimento.Text = "Pavimento " + pavSelecionado;
                lbfck.Text = fckSelecionado;
                lbEci.Text = Convert.ToString(calculos.calculaDEci(fck, aE));
                lbEcs.Text = Convert.ToString(calculos.calculaDEcs(fck, aE));
                lbfck.Visible = true;
                lbEci.Visible = true;
                lbEcs.Visible = true;
                try
                {
                    preencheTabelaResistencia(s, fck);
                    plotaGrafico(dadosGrafico);
                }
                catch { }
                carregaTodosCobrimentos();
            }
            catch { }
        }

        private void dgvGerais_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int linha = dgvGerais.SelectedCells[0].RowIndex;
                int coluna = dgvGerais.SelectedCells[0].ColumnIndex;
                string novoValor = "";
                double numero = 0;
                if (coluna == 1)
                {
                    pavimentoEditar = this.dgvGerais.Rows[linha].Cells[0].Value.ToString();
                    string nomeGroupBox = "Pavimento " + pavimentoEditar;
                    if (nomeGroupBox == gbPavimento.Text)
                    {
                        novoValor = this.dgvGerais.Rows[linha].Cells[1].Value.ToString();
                        numero = funcoes.converteEmNumero(novoValor);
                        indicaMudancaFckTabela = 1;
                        atualizaFck(pavimentoEditar, novoValor);
                        string caa = cbClasseAgressividade.Text;
                        string aviso = calculos.mensagemAlertaFck(numero, caa);
                        carregaTodosCobrimentos();
                    }
                }
                if (novoValor != "" && numero > 0 && lbfck.Text != "FCK")
                {
                    lbfck.Text = novoValor;
                }
                //MessageBox.Show(novoValor, "Novo valor");
            }
            catch { }
        }

        private void lbfck_TextChanged(object sender, EventArgs e)
        {
            if (indicaMudancaFckTabela == 1)
            {
                try
                {
                    string fckSelecionado = lbfck.Text;
                    double fck = funcoes.converteEmNumero(fckSelecionado);

                    string tipoAgregado = cbTipoAgregado.Text;
                    double aE = alfaE(tipoAgregado);
                    s = funcoes.converteEmNumero(lbS.Text);
                    lbEci.Text = Convert.ToString(calculos.calculaDEci(fck, aE));
                    lbEcs.Text = Convert.ToString(calculos.calculaDEcs(fck, aE));
                    lbfck.Visible = true;
                    lbEci.Visible = true;
                    lbEcs.Visible = true;
                    try
                    {
                        preencheTabelaResistencia(s, fck);
                        plotaGrafico(dadosGrafico);
                    }
                    catch { }
                    //carregaTodosCobrimentos();
                }
                catch { }
                indicaMudancaFckTabela = 0;
            }
            try
            {
                double resi = funcoes.converteEmNumero(lbfck.Text);
                string caa = cbClasseAgressividade.Text;
                string msg = calculos.mensagemAlertaFck(resi, caa);
                labelAdvertencia.Text = msg;
                carregaTodosCobrimentos();

            }
            catch { }
        }

        private void atualizaFck(string pavimento, string sfck)
        {
            try
            {
                int num = lstTodosDadosTabela.Count();
                for (int i = 0; i < num; i = i + 16)
                {
                    if (pavimento == lstTodosDadosTabela[i])
                    {
                        lstTodosDadosTabela[i + 1] = sfck;
                    }
                }
            }
            catch { }
            
        }

        private void resetar()
        {
            indicaMudancaFckTabela = 0;
            dgvGerais.Rows.Clear();
            dgvQuantitativo.Rows.Clear();
            lbPavimentos.Items.Clear();
            lbPavimentosSelecionados.Items.Clear();
            txtVf.Text = "";
            txtPf.Text = "";
            txtLf.Text = "";
            txtVc.Text = "";
            txtPc.Text = "";
            txtLc.Text = "";
            lstTodosDadosTabela.Clear();
            listaMarcacoes.Clear();
        }

        private void organizarListBoxPavimentos()
        {
            List<string> lstOrdenada = new List<string>();
            List<string> lstTemp = new List<string>();
            bool existe = false;
            try
            {
                foreach (string pavi in lbPavimentosSelecionados.Items) { lstTemp.Add(pavi); }

                foreach (string pav in lstPavimentos)
                {
                    existe = lstTemp.Any(s => s == pav);
                    if(existe==true)
                    {
                        lstOrdenada.Add(pav);
                    }
                }
                lbPavimentosSelecionados.Items.Clear();
                foreach(string p in lstOrdenada) { lbPavimentosSelecionados.Items.Add(p); }
                
            }
            catch { }
            //int oooo = 0;
        }

        private void preencheCabecalhoTabelas()
        {
            this.dgvGerais.Columns[6].HeaderText = "Área Estruturada (m2)";
            this.dgvGerais.Columns[7].HeaderText = "Área de Formas (m2)";
            this.dgvGerais.Columns[8].HeaderText = "Volume de concreto (m3)";
            this.dgvQuantitativo.Columns[7].HeaderText = "Volume EPS Nervuradas (m3)";
        }

        private void gbPavimento_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string fckSelecionado = lbfck.Text;
                double fck = funcoes.converteEmNumero(fckSelecionado);

                string tipoAgregado = cbTipoAgregado.Text;
                double aE = alfaE(tipoAgregado);
                s = funcoes.converteEmNumero(lbS.Text);
                lbEci.Text = Convert.ToString(calculos.calculaDEci(fck, aE));
                lbEcs.Text = Convert.ToString(calculos.calculaDEcs(fck, aE));
                lbfck.Visible = true;
                lbEci.Visible = true;
                lbEcs.Visible = true;
                try
                {
                    preencheTabelaResistencia(s, fck);
                    plotaGrafico(dadosGrafico);
                }
                catch { }
                //carregaTodosCobrimentos();
            }
            catch { }
            indicaMudancaFckTabela = 0;
        
            try
            {
                double resi = funcoes.converteEmNumero(lbfck.Text);
                string caa = cbClasseAgressividade.Text;
                string msg = calculos.mensagemAlertaFck(resi, caa);
                labelAdvertencia.Text = msg;
                carregaTodosCobrimentos();

            }
            catch { }
        }

        private void carregarInformacoesProjeto()
        {
            txtNumeroProjeto.Text = dadosProjeto[0];
            txtTituloEdificio.Text = dadosProjeto[1];
            txtTituloCliente.Text = dadosProjeto[2];
        }

        private void lbPavimentos_SelectedIndexChanged(object sender, EventArgs e)
        {
            tipoMensagem = 1;
            aparecerMensagem(tipoMensagem);
        }

        private void aparecerMensagem(int indicador)
        {
            string mensagem = "";
            switch (indicador)
            {
                case 1:
                    mensagem = "Lista de pavimentos existentes no Edifício.";
                    break;
                case 2:
                    mensagem = "Lista de pavimentos selecionados pelo usuário";
                    break;
                case 3:
                    mensagem = "Número que identifica o projeto";
                    break;
                case 4:
                    mensagem = "Identificação do Edifício";
                    break;
                case 5:
                    mensagem = "Identificação do cliente";
                    break;
            }
            txtDescricao.Text = mensagem;
        }

        private void txtNumeroProjeto_Click(object sender, EventArgs e)
        {
            tipoMensagem = 3;
            aparecerMensagem(tipoMensagem);
        }

        private void txtTituloEdificio_Click(object sender, EventArgs e)
        {
            tipoMensagem = 4;
            aparecerMensagem(tipoMensagem);
        }

        private void txtTituloCliente_Click(object sender, EventArgs e)
        {
            tipoMensagem = 5;
            aparecerMensagem(tipoMensagem);
        }

        private void bt_Pan_Click(object sender, EventArgs e)
        {
            try { this.dc.Pan(); }
            catch { }
        }

        private void bt_ZoomWindow_Click(object sender, EventArgs e)
        {
            try { this.dc.ZoomWindow(); }
            catch { }
        }

        private void bt_ZoomOut_Click(object sender, EventArgs e)
        {
            try { this.dc.ZoomOut(); }
            catch { }
        }

        private void bt_ZoomExtents_Click(object sender, EventArgs e)
        {
            try { this.dc.ZoomExtents(); }
            catch { }
        }

        private void btSalvaDwg_Click(object sender, EventArgs e)
        {
            //if (caminhoArquivoDwg != "")
            //{
            //    string[] infos = constroiNovasInformacoes();
            //    desenho.preencheDesenhoDxf(caminhoArquivoDwg, infos);
            //    this.dc.Save(caminhoArquivoDwg);
            //}
            
            //salva o arquivo atual
            // Salva o desenho de acordo com o formato desejado
            SaveFileDialog caixaDialogoSalvar = new SaveFileDialog();
            caixaDialogoSalvar.Filter = "Arquivo DXF|*.dxf";
            caixaDialogoSalvar.Title = "Salvar desenho";
            DialogResult resultado = caixaDialogoSalvar.ShowDialog();
            //caixaDialogoSalvar.FileName = "RESUMO_MATERIAL";
            string nomeArquivo = caixaDialogoSalvar.FileName;
            if (caixaDialogoSalvar.FileName != "" && resultado == DialogResult.OK)
            {
                string diretorioOriginal = caminhoArquivoDwg;
                if (System.IO.File.Exists(nomeArquivo) == true)
                {
                    MessageBox.Show("Olá");
                    System.IO.File.Delete(nomeArquivo);

                }
                
                System.IO.File.Copy(diretorioOriginal, nomeArquivo);
                string[] infos = constroiNovasInformacoes();
                desenho.preencheDesenhoDxf(nomeArquivo, infos);
                this.dc.Save(nomeArquivo);

                this.dc.Open(nomeArquivo);
                this.dc.ZoomExtents();

                File.Delete(pegaArquivoDwg(nomeArquivo));
                File.Delete(pegaArquivoBak(nomeArquivo));
                if(caminhoArquivoTmp!="")
                {
                    deletarArquivoTemporario();
                }
                
            }
            
        }

        //private void btAbrirDwg_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog caixaDialogo = new OpenFileDialog();
        //    caixaDialogo.InitialDirectory = "C:\\";
        //    caixaDialogo.Title = "Selecione um arquivo dxf";
        //    caixaDialogo.Filter = "Arquivos dxf (*.dxf)|*.dxf";
        //    caixaDialogo.ShowDialog();
        //    caminhoArquivoDwg = caixaDialogo.FileName;
        //    if (caminhoArquivoDwg != null)
        //    {
        //        this.dc.Open(caminhoArquivoDwg);
        //        this.dc.ZoomExtents();
        //    }
        //
        //}

        private void abrirDesenhoDxf()
        {
            caminhoArquivoDwg = "C:\\TQSW\\SUPORTE\\DP\\DPS\\QINLOCO\\RESUMO_MATERIAL.DXF";
            try
            {
                this.dc.Open(caminhoArquivoDwg);
                this.dc.ZoomExtents();
            }
            catch { MessageBox.Show("Não foi possível encontrar o arquivo DXF", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string[] constroiNovasInformacoes()
        {
            string caa = cbClasseAgressividade.Text;
            string[] quantitativo = new string[10];
            quantitativo = pegaDadosQuantitativo(lbPavimento.Text);
            string[] info = new string[48];
            info[0] = lbfck.Text;
            info[1] = lbEci.Text;
            info[2] = lbEcs.Text;
            info[3] = cbClasseAgressividade.Text;

            if (caa == "I-Fraca-Rural")
            {
                info[4] = "<=0,65";
                info[5] = "";
                info[6] = "";
                info[7] = "";
                info[8] = "<=0,60";
                info[9] = "";
                info[10] = "";
                info[11] = "";
                info[12] = ">=260";
                info[13] = "";
                info[14] = "";
                info[15] = "";
            }
            else if(caa== "II-Moderada-Urbana")
            {
                info[4] = "";
                info[5] = "<=0,60";
                info[6] = "";
                info[7] = "";
                info[8] = "";
                info[9] = "<=0,55";
                info[10] = "";
                info[11] = "";
                info[12] = "";
                info[13] = ">=280";
                info[14] = "";
                info[15] = "";
            }
            else if(caa== "III-Forte-Marinho")
            {
                info[4] = "";
                info[5] = "";
                info[6] = "<=0,55";
                info[7] = "";
                info[8] = "";
                info[9] = "";
                info[10] = "<=0,50";
                info[11] = "";
                info[12] = "";
                info[13] = "";
                info[14] = ">=320";
                info[15] = "";
            }
            else if (caa == "IV-Muito forte-Industrial")
            {
                info[4] = "";
                info[5] = "";
                info[6] = "";
                info[7] = "<=0,45";
                info[8] = "";
                info[9] = "";
                info[10] = "";
                info[11] = "<=0,45";
                info[12] = "";
                info[13] = "";
                info[14] = "";
                info[15] = ">=360";
            }
            string R7 = txtR7.Text;
            string R14 = txtR14.Text;
            string R21 = txtR21.Text;
            string Eci7 = txtEci7.Text;
            string Eci14 = txtEci14.Text;
            string Eci21 = txtEci21.Text;

            info[16] = R7;
            info[17] = R14;
            info[18] = R21;
            info[19] = Eci7;
            info[20] = Eci14;
            info[21] = Eci21; 
            info[22] = txt1.Text + " cm";
            info[23] = txt3.Text + " cm";
            info[24] = txt5.Text + " cm";
            info[25] = txt7.Text + " cm";
            info[26] = txt9.Text + " cm";
            info[27] = txt11.Text + " cm";
            info[28] = txt13.Text + " cm";
            info[29] = txt15.Text + " cm";
            info[30] = txt17.Text + " cm";
            info[31] = txt19.Text + " cm";
            info[32] = txt21.Text + " cm";
            info[33] = txt23.Text + " cm";
            info[34] = txt25.Text + " cm";
            if (cbControleQualidade.Checked == true)
            {
                info[35] = "DEVE SER ADOTADO CONTROLE DE QUALIDADEE RÍGIDOS LIMITES DE";
                info[36] = "TOLERÂNCIA DA VARIABILIDADE DAS MEDIDAS DURANTE A EXECUÇÃO";
            }
            else if (cbControleQualidade.Checked == false)
            {
                info[35] = "";
                info[36] = "";
            }
            info[37] = lbPavimento.Text;
            info[38] = quantitativo[0];
            info[39] = quantitativo[1];
            info[40] = quantitativo[2];
            info[41] = quantitativo[3];
            info[42] = quantitativo[4];
            info[43] = quantitativo[5];
            info[44] = quantitativo[6];
            info[45] = quantitativo[7];
            info[46] = quantitativo[8];
            info[47] = quantitativo[9];

            return info;
        }

        private string[] pegaDadosQuantitativo(string pavi)
        {
            string[] dados = new string[10];
            foreach (DataGridViewRow row in dgvQuantitativo.Rows)
            {
                if (Convert.ToString(row.Cells[0].Value) != "")
                {
                    
                    if (row.Cells[0].Value.ToString() == pavi)
                    {
                        dados[0] = Convert.ToString(row.Cells[4].Value);
                        dados[1] = Convert.ToString(row.Cells[5].Value);
                        dados[2] = Convert.ToString(row.Cells[6].Value);
                        
                        dados[5] = Convert.ToString(row.Cells[1].Value);
                        dados[6] = Convert.ToString(row.Cells[2].Value);
                        dados[7] = Convert.ToString(row.Cells[3].Value);
                    }
                }
            }
            foreach (DataGridViewRow row in dgvGerais.Rows)
            {
                if (Convert.ToString(row.Cells[0].Value) != "")
                {

                    if (row.Cells[0].Value.ToString() == pavi)
                    {
                        dados[3] = Convert.ToString(row.Cells[8].Value);
                        dados[4] = Convert.ToString(row.Cells[5].Value);

                        dados[8] = Convert.ToString(row.Cells[7].Value);
                        dados[9] = Convert.ToString(row.Cells[6].Value);
                    }
                }
            }

            return dados;
        }

        private void habilitaComandosDesenho()
        {
            //btAbrirDwg.Enabled = true;
            btSalvaDwg.Enabled = true;
            bt_ZoomExtents.Enabled = true;
            bt_ZoomOut.Enabled = true;
            bt_ZoomWindow.Enabled = true;
            bt_Pan.Enabled = true;
        }

        private void desabilitaComandosDesenho()
        {
            //btAbrirDwg.Enabled = false;
            btSalvaDwg.Enabled = false;
            bt_ZoomExtents.Enabled = false;
            bt_ZoomOut.Enabled = false;
            bt_ZoomWindow.Enabled = false;
            bt_Pan.Enabled = false;
        }

        private string nomeArquivoDwg(string nomeArquivo)
        {
            string[] dados = nomeArquivo.Split('.');
            int n = dados.Count();
            dados[n - 1] = ".DWG";
            string arquivoDwg = "";
            for (int i = 0; i < n; i++)
            {
                arquivoDwg = arquivoDwg + dados[i];
            }
            return arquivoDwg;
        }

        private string pegaArquivoDwg(string nomeArquivo)
        {
            string[] dados = nomeArquivo.Split('.');
            int n = dados.Count();
            dados[n - 1] = ".DWG";
            string arquivoDwg = "";
            for (int i = 0; i < n; i++)
            {
                arquivoDwg = arquivoDwg + dados[i];
            }
            return arquivoDwg;
        }

        private string pegaArquivoBak(string nomeArquivo)
        {
            string[] dados = nomeArquivo.Split('.');
            int n = dados.Count();
            dados[n - 1] = ".BAK";
            string arquivoBak = "";
            for (int i = 0; i < n; i++)
            {
                arquivoBak = arquivoBak + dados[i];
            }
            return arquivoBak;
        }

        private void btAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (caminhoArquivoTmp != "")
                {
                    deletarArquivoTemporario();
                    criarArquivoTemporario();
                    string[] infos = constroiNovasInformacoes();
                    desenho.preencheDesenhoDxf(caminhoArquivoTmp, infos);
                    this.dc.Save(caminhoArquivoTmp);

                    this.dc.Open(caminhoArquivoTmp);
                    this.dc.ZoomExtents();

                    File.Delete(pegaArquivoDwg(caminhoArquivoTmp));
                    File.Delete(pegaArquivoBak(caminhoArquivoTmp));
                }
            }
            catch { }
        }

        private void criarArquivoTemporario()
        {
          caminhoArquivoTmp = "C:\\TQSW\\SUPORTE\\DP\\DPS\\QINLOCO\\RESUMO_MATERIAL_TEMP.DXF";
          System.IO.File.Copy(caminhoArquivoDwg, caminhoArquivoTmp); 
        }

        private void deletarArquivoTemporario()
        {
            try
            {
                File.Delete(caminhoArquivoTmp);
            }
            catch { }
            caminhoArquivoTmp = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
