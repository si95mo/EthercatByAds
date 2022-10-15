Imports EthercatByAds

''' <summary>
''' Ads DLL example application in VB
''' </summary>
Module Program
    ''' <summary>
    ''' The AMS net id address. Leave empty to connect to a local ADS server, otherwise specify the in the correct route
    ''' </summary>
    Const AmsNetId As String = ""

    ''' <summary>
    ''' Application entry-point
    ''' </summary>
    Sub Main()
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Start{Environment.NewLine}")

        Try
            '' Initialization
            Dim initialized As Boolean = Ads.Initialize(AmsNetId, 851).Result
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Initialization done: {initialized}")
            If Ads.IsInError Then
                Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Communication in error")
                Console.WriteLine($"{vbTab}{vbTab}>> Reason of failure: {Ads.ReasonOfFailure}")
            End If

            '' Write operations
            Dim digitalWriteDone As Boolean = Ads.Write("DigitalOutput", True) '' Digital write - the return value is the succeeded status
            Console.WriteLine($"{vbTab}{vbTab}>> Digital write done? {digitalWriteDone}")

            Dim analogWriteDone As Boolean = Ads.Write("AnalogOutput", Math.PI) '' Analog write - the return value is the succeeded status
            Console.WriteLine($"{vbTab}{vbTab}>> Analog write done? {analogWriteDone}")

            '' Read operations
            Dim digitalReadValue As Boolean
            Dim firstAnalogReadValue As Double

            Dim digitalReadDone As Boolean = Ads.Read("DigitalInput", digitalReadValue) '' Digital read - the return value is the succeeded status. The read value is inside the variable
            Dim analogReadDone As Boolean = Ads.Read("AnalogInput", firstAnalogReadValue) '' Analog read - the return value is the succeeded status. The read value is inside the variable

            '' Value read from the ADS server
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Digital read value: {digitalReadValue}")
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Analog read value: {firstAnalogReadValue}")
        Catch ex As Exception
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Error occurred: {Environment.NewLine}{vbTab}{vbTab}>> {ex.Message}")
        End Try

        Console.WriteLine($"{Environment.NewLine}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> End{Environment.NewLine}")

        '' End of communication
        Dim stopResult As Boolean = Ads.Stop()
        Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Stop result: {stopResult}")

        Console.WriteLine("------------------------------------------------------------")
        Console.Write("Press 'ENTER' to exit the application: ")
        Console.ReadLine()
    End Sub
End Module
