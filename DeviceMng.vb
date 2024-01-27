Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Management
Imports System.ComponentModel
Public Class DeviceMng

    Private _VisualizzaDettagliNodo As Boolean = True
    <Browsable(True),
    Category("Custom Properties"),
    Description("Visualizza Dettagli Nodo. True per mostrare, False per nascondere."),
    TypeConverter(GetType(BooleanConverter))>
    Public Property VisualizzaDettagliNodo As Boolean
        Get
            Return _VisualizzaDettagliNodo
        End Get
        Set(value As Boolean)
            _VisualizzaDettagliNodo = value
            LabelInfo.Visible = value
        End Set
    End Property
    Private _RetrieveDeviceInformation As Boolean = True
    Private _deviceInformation As Dictionary(Of String, List(Of String))
    <Browsable(True),
Category("Custom Properties"),
Description("Ottiene le informazioni sui dispositivi. True per eseguire la query, False per utilizzare i dati esistenti.")>
    Public Property RetrieveDeviceInformation As Boolean
        Get
            Return _RetrieveDeviceInformation
        End Get
        Set(value As Boolean)
            _RetrieveDeviceInformation = value
            If value Then
                ' Esegui la query per ottenere le informazioni sui dispositivi
                _deviceInformation = GetDeviceInformation()
            End If
        End Set
    End Property
    Private Function GetDeviceInformation() As Dictionary(Of String, List(Of String))
        ' Ottieni l'elenco dei dispositivi connessi al PC
        Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity")
        Dim categories As New Dictionary(Of String, List(Of String))

        ' Scansiona i risultati e organizza per categorie
        For Each device As ManagementObject In searcher.Get()
            Dim category As String = TryCast(device("PNPClass"), String)
            Dim name As String = TryCast(device("Caption"), String)

            If category IsNot Nothing AndAlso name IsNot Nothing Then
                If Not categories.ContainsKey(category) Then
                    categories.Add(category, New List(Of String)())
                End If

                categories(category).Add(name)
            End If
        Next

        ' Popola il TreeView con le categorie e i dispositivi
        For Each category In categories
            Dim categoryNode As New TreeNode(category.Key)
            For Each deviceName In category.Value
                categoryNode.Nodes.Add(deviceName)
            Next
            ' Assuming TreeView1 is the name of your TreeView control
            TreeView1.Nodes.Add(categoryNode)
        Next

        ' Return the categories
        Return categories
    End Function

    ' Gestore dell'evento chiamato quando un nodo viene selezionato nel TreeView
    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        ' Controlla se è stato selezionato un nodo categoria o un nodo dispositivo
        If e.Node.Parent Is Nothing Then
            ' Nodo categoria selezionato
            ShowCategoryInfo(e.Node)
        Else
            ' Nodo dispositivo selezionato
            ShowDeviceInfo(e.Node)
        End If
    End Sub

    ' Visualizza informazioni sulla categoria selezionata nel Label multiline
    Private Sub ShowCategoryInfo(node As TreeNode)
        ' Visualizza il conteggio dei dispositivi nella categoria
        Dim deviceCount As Integer = node.Nodes.Count
        DisplayDeviceInfo($"Numero di dispositivi nella categoria '{vbCrLf}{node.Text}': {deviceCount}")
    End Sub

    ' Visualizza informazioni dettagliate sul dispositivo selezionato nel Label multiline
    Private Sub ShowDeviceInfo(node As TreeNode)
        ' Visualizza informazioni dettagliate sul dispositivo
        DisplayDeviceInfo($"Dettagli del dispositivo:{vbCrLf}{node.Text}")
    End Sub

    ' Visualizza le informazioni nel Label multiline
    Private Sub DisplayDeviceInfo(info As String)
        ' Visualizza le informazioni nel Label multiline
        LabelInfo.Text = info
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        GetDeviceInformation()
    End Sub

End Class
