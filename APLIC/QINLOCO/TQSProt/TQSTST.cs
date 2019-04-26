using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace TQS
{
    partial class TQSTST
    {
        private static bool tstext = false;
        private static string CRCCUSTOMDL()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "hash.exe";
            cmd.StartInfo.Arguments = "CUSTOMDL.DLL /sha1 /c";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.Start();
            cmd.WaitForExit();
            return cmd.StandardOutput.ReadLine();
        }
        //
        public static bool isAppEnvironment()
        {
            return true;
        }
        //
        public static double delta()
        {
            //
            int istat = -1;
            //
            //	Define um código do aplicativo não padrão. 
            //	- 8 caracteres com preenchimento de brancos à direita ("TSTAPLIC")
            //
            StringBuilder aplicnp = new StringBuilder("QINLOCO", 8);
            TQS.APLICTQS.VERFCBAPNP(aplicnp, 0);
            //
            //	Para comunicação com o NULF.EXE é necessário que a pasta de 
            //	temporários esteja livre para gravação
            //
            StringBuilder dirtemp = new StringBuilder("QINLOCO", 256);
            TQS.APLICTQS.VERFCBACEGRV(dirtemp, 0, ref istat);
            if (istat != 0)
            {
                string str = "Não é possível gravar arquivos na pasta [%s].\n" +
                             "É necessario que voce tenha permissão total " +
                             "de acesso a esta pasta para instalar e rodar " +
                             "este aplicativo.\n";
                MessageBox.Show(str);
                return -1.0;
            }
            //
            //	Verifica a existência de updates. Sai se o usuário pediu para
            //	executar um update
            //
            int iupdate = 0;
            TQS.APLICTQS.VERFCBUPDT(ref iupdate);
            if (iupdate != 0) return -1.0;
            //
            //	Verifica integridade do NULF.EXE (Utilitário de Licenças) 
            //	Cai fora se não tiver integridade
            //
            TQS.APLICTQS.VERFCBTCNLF();
            //
            //	Se a licença expirou por falta de chamar o programa e consultar a
            //	Web, faz uma chamada direta ao NULF e termina o programa
            //
            TQS.APLICTQS.VERFCBRVAL(ref istat);
            if (istat != 0) System.Environment.Exit(istat);
            //
            //	Protocolo de extensão de validade da licença. Chama o NULF em 
            //	paralelo, e ele vai deixar um arquivo na pasta de temporários.
            //	Este arquivo será usado na próxima chamada deste aplicativo,
            //	para revalidar ou estender a licença.
            //	
            TQS.APLICTQS.VERFCBRVL();
            //
            //	Teste da licença
            //
            TQS.APLICTQS.VERFCBPMM(ref istat);
            //
            //	A licença Ok consiste no código ERRMM_ISTATOK retornado pela rotina
            //	acima. Para confundir um pouco um atacante, faz com que o código
            //	de erro faça com que o programa erre as contas
            //
            if (istat - TQS.APLICTQS.ERRMM_ISTATOK != 0.0) return -3.0;
            //
            //	Auto teste da CUSTOMDL.DLL.
            //
            TQS.APLICTQS.VERFCBAUTOCUS(ref istat);
            if (istat - 4974 != 0.0) return -4.0;
            //
            //	Se o NULF verificou validade ou estendeu a licença, faz a extensão
            //	Status de retorno somente informativo. Em caso de erro, 
            //	alguma hora a licença web vai parar.
            //
            if (tstext == false)
            { 
                TQS.APLICTQS.VERFCBEXTAUTO(ref istat);
                tstext = true;
            }
            //
            //  Verifica se está rodando em APP ou dentro do TQS
            //
            if (isAppEnvironment())
            {
                //
                //	Verifica integridade da CUSTOMDL
                //
                if (CRCCUSTOMDL() == HASH.CUSTOMDL_HASH)
                {
                    //
                    // Caso o CRC bata com a CUSTOMDL, significa que está rodando via app, então testo a assinatura 1
                    //
                    StringBuilder name = new StringBuilder("QINLOCO.EXE");
                    TQS.APLICTQS.VERFCBTCRC(name, 0, TQS.TQSTST.glb_chapubl, ref istat);
                    if (istat != 0.0) return -5.0;
                }
                else
                {
                    return -6.0;
                }
            }
            else
            {
                //
                //  Significa que está rodando via Gerenciador, então testo a assinatura 2
                //
                istat = 0;
                TQS.APLICTQS.VERFCBAUTOX(ref istat);
                if (istat != 0.0) return -7.0;
            }
            // 
            // Nome do aplicativo para a biblioteca de acesso a dados do edificio
            // 
            TQS.APLICTQS.ACLTQSDEFAPLICNP(aplicnp, 0);
            //
            //  Nome do aplicativo para a biblioteca de desenho
            //
            TQS.APLICTQS.g_verfcbpnp(aplicnp, 0);
            //
            return (double)istat;
        }
        //
        public static double delta2()
        {
            int istat = 1;
            //
            //  Verifica se está rodando em APP ou dentro do TQS
            //
            if (isAppEnvironment())
            {
                //
                //	Verifica integridade da CUSTOMDL
                //
                if (CRCCUSTOMDL() == HASH.CUSTOMDL_HASH)
                {
                    //
                    // Caso o CRC bata com a CUSTOMDL, significa que está rodando via app, então testo a assinatura 1
                    //
                    StringBuilder name = new StringBuilder("QINLOCO.EXE");
                    TQS.APLICTQS.VERFCBTCRC(name, 0, TQS.TQSTST.glb_chapubl, ref istat);
                    if (istat != 0.0) return 1000.0;
                }
                else
                {
                    return 1000.0;
                }
            }
            else
            {
                //
                //  Significa que está rodando via Gerenciador, então testo a assinatura 2
                //
                istat = 0;
                TQS.APLICTQS.VERFCBAUTOX(ref istat);
                if (istat != 0.0) return 1000.0;
            }
            //
            return 1.0;
        }
        //
        public static double delta3()
        {
            int istat = -1;
            TQS.APLICTQS.VERFCBAUTOCUS(ref istat);
            if (istat - 4974 == 0.0) return 1.0;
            return 0.001;
        }
    }
}
