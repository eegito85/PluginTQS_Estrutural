using System.Runtime.InteropServices;

namespace TQS
{ 
    public class APLICTQS
    {
        public const int ERRMM_ISTATOK = 14;
        //
        //	Nome do aplicativo não padrão definido antes do teste da licença
        //	Exemplo: "APLIC   "	(8 caracteres, brancos à direita)
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBAPNP(System.Text.StringBuilder aplicnp, int ARG_FAN);
        //
        //	Teste necessário antes da chamada do NULF .EXE
        //	(Utilitário de Licenças Flutuantes)
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBTCNLF();
        //
        //	Execução do NULF.EXE em paralelo. 
        //	
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBNLF(ref int ibatch);
        //
        //	Teste da licença. Retorna istat==ERRMM_ISTATOK se Ok
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBPMM(ref int istat);
        //
        //	Verifica se o usuário tem direito de gravação na pasta de 
        //	temporários. Esta pasta está definida no arquivo GEREN.DAT
        //	distribuída na pasta $USUARIO\NGE
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBACEGRV(System.Text.StringBuilder aplicnp, int ARG_FAN, ref int istat);
        //
        //	Consulta de validade da licença no servidor. Chama o NULF em
        //	paralelo, e vai largar arquivos na pasta temporária que só serão
        //	interpretados no dia seguinte. A não chamada desta rotina causa
        //	invalidação da licença após número de dias definidos na licença.
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBRVL();
        //
        //	Se o NULF consultou o servidor, no dia seguinte a rotina abaixo
        //	atualiza a licença para estender sua validade ou atualizar os
        //	recursos do usuario
        // -> (0) Nenhuma atualização ocorreu
        // -> (1) Licença verificada
        // -> (2) Licença atualizada
        // -> (3) NULF chamado com função inválida
        // -> (4) Erro de comunicação c/o servidor
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBEXTAUTO(ref int istat);
        //
        //	Teste da licença que chama o NULF diretamente caso o usuário tenha
        //	passado muito tempo sem usar o programa
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBRVAL(ref int istat);
        //
        //	Testa de integridade: se um arquivo contém assinatura válida.
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBTCRC(System.Text.StringBuilder aplic, int ARG_FAN, byte[] chavepubl, ref int istat);
        //
        //	Testa a integridade da CUSTOM.DLL
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBAUTOCUS(ref int istat);
        //
        //	Testa integridade do aplicativo TQS. Se Ok, istat = istat - 26
        //	A assinatura tem que ser feita na TQS
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBAUTOX(ref int istat);
        //
        //	Chama verificador de atualização em paralelo. Deixará um arquivo
        //	instalável que será executado na próxima execução
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBUPDT(ref int iupdate);
        //
        //	Retorna número e nome dos aplicativos não padrão disponíveis
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBAPNPNU(ref int napnp);
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBAPNPNO(ref int iaplic, System.Text.StringBuilder aplic, int ARG_FAN);
        //
        //	Retorna pasta de programas do aplicativo
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBTQSPROG(System.Text.StringBuilder aplicnp, int ARG_FAN, ref int istat);
        //
        //	Retorna pasta de suporte do aplicativo
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBTQSSUPOR(System.Text.StringBuilder aplicnp, int ARG_FAN, ref int istat);
        //
        //	Retorna pasta de usuário do aplicativo
        //
        [DllImport("CUSTOMDL.DLL")]
        public static extern void VERFCBTQSPREFER(System.Text.StringBuilder aplicnp, int ARG_FAN, ref int istat);
        //
        // Nome de aplicativo não padrão para a biblioteca de desenho
        // 
        [DllImport("MDWG.DLL")]
        public static extern void g_verfcbpnp(System.Text.StringBuilder aplicnp, int ARG_FAN);
        //
        // Nome de aplicativo não padrão para a biblioteca de acesso aos dados do edifício
        // 
        [DllImport("ACESSOL.DLL")]
        public static extern void ACLTQSDEFAPLICNP(System.Text.StringBuilder aplicnp, int ARG_FAN);
        //
    }
}
