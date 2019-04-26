''---------------------------------------------------------------------------
''   Arquivo de chave RSA 1024
''   Gravado em [C:\TQSAPP\QINLOCO\APLIC\QINLOCO\TQSProt\QINLOCO_EXE_CPU.H] em 21-09-2017 17:11:13
''   Pelo programa TQSASSIN.EXE
''   Chave pública correspondente ao arquivo [C:\TQSAPP\QINLOCO\APLIC\QINLOCO\TQSProt\QINLOCO.EXE]
''---------------------------------------------------------------------------
Public Const glb_nchapubl As Integer = 160
Public Sub glb_chapubl(chapubl() As Byte)
    Dim theArray() As Variant
    theArray() = Array( _
	&H30, &H81, &H9D, &H30, &H0D, &H06, &H09, &H2A, &H86, &H48,  _
	&H86, &HF7, &H0D, &H01, &H01, &H01, &H05, &H00, &H03, &H81,  _
	&H8B, &H00, &H30, &H81, &H87, &H02, &H81, &H81, &H00, &HB9,  _
	&HC8, &H05, &H15, &H6B, &H16, &H36, &H45, &H15, &H86, &H0E,  _
	&HFB, &H94, &H73, &H8F, &H64, &HED, &H56, &H71, &HD3, &HFC,  _
	&H3C, &H4F, &HF5, &H65, &HD5, &HE7, &HFE, &HC4, &H2C, &H3D,  _
	&H0F, &HDA, &H3F, &HD3, &H64, &H8A, &HC7, &H49, &H2E, &H4E,  _
	&HD7, &HF1, &H64, &HA8, &H48, &HCB, &H5A, &H2C, &HD5, &H9A,  _
	&HB3, &HF3, &HFD, &H8C, &H2E, &HDE, &H6A, &H86, &H19, &HF0,  _
	&HEF, &H43, &HBD, &HC1, &H46, &HAB, &H94, &HD7, &HB8, &H33,  _
	&H9E, &H80, &H34, &H85, &H6F, &HBA, &HB8, &H5A, &HF1, &HC0,  _
	&H11, &H39, &H4F, &HC9, &H45, &H0E, &H27, &H0C, &H86, &H5C,  _
	&HCE, &HD6, &HD5, &H1B, &H5A, &HE4, &H62, &H73, &H9F, &H89,  _
	&HFF, &H71, &HB0, &HAC, &HD8, &H7F, &HD7, &H7B, &H62, &H42,  _
	&HA4, &H89, &HBE, &HE6, &HBB, &H44, &HF7, &H7E, &HF6, &H3B,  _
	&H1A, &H1D, &HC9, &HD1, &HD3, &HC7, &H57, &H02, &H01, &H11   _
    )
    ReDim chapubl(0 To glb_nchapubl - 1)
    Dim i As Integer
    For i = 0 To glb_nchapubl - 1
        chapubl(i) = theArray(i)
    Next
End Sub
