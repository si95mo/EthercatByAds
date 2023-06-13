Imports EthercatByAds
Imports System.Threading

''' <summary>
''' Ads DLL example application in VB
''' </summary>
Module Program
    ''' <summary>
    ''' The AMS net id address. Leave empty to connect to a local ADS server, otherwise specify the in the correct route
    ''' </summary>
    Const AmsNetId As String = "5.112.56.172.1.1"

    ''' <summary>
    ''' Application entry-point
    ''' </summary>
    Sub Main()
        Console.WriteLine("{0} >> Start{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), Environment.NewLine)

        Try
            ' Initialization
            Dim initialized As Boolean = Ads.Initialize(AmsNetId, 851).Result
            Console.WriteLine("{0}{1} >> Initialization done: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), initialized)
            If Ads.IsInError Then
                Console.WriteLine("{0}{1} >> Communication in error", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"))
                Console.WriteLine("{0}{1}>> Reason of failure: {2}", vbTab, vbTab, Ads.ReasonOfFailure)
            End If

            '' Write operations
            'Dim digitalWriteDone As Boolean = Ads.Write("DigitalOutput", True) '' Digital write - the return value is the succeeded status
            'Console.WriteLine($"{vbTab}{vbTab}>> Digital write done? {digitalWriteDone}")

            'Dim analogWriteDone As Boolean = Ads.Write("AnalogOutput", Math.PI) '' Analog write - the return value is the succeeded status
            'Console.WriteLine($"{vbTab}{vbTab}>> Analog write done? {analogWriteDone}")

            ' Read operations
            Dim digitalReadValue As Boolean
            Dim analogReadValue As Double

            Dim digitalReadDone As Boolean

            digitalReadDone = Ads.Read("SYS_OK_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> SYS_OK_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            digitalReadDone = Ads.Read("ALARM_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> ALARM_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            digitalReadDone = Ads.Read("OPEN_COLLET_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> OPEN_COLLET_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            digitalReadDone = Ads.Read("HOOKED_TOOL_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> HOOKED_TOOL_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            digitalReadDone = Ads.Read("NO_TOOL_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> NO_TOOL_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            digitalReadDone = Ads.Read("FAN_ROTATING_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> FAN_ROTATING_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            digitalReadDone = Ads.Read("THERMAL_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> THERMAL_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            digitalReadDone = Ads.Read("PISTON_BACK_BIT", digitalReadValue) ' Digital read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> PISTON_BACK_BIT: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), digitalReadValue)

            Dim allDigitals() As Boolean
            allDigitals = Ads.ReadBits()

            Dim analogReadDone As Boolean

            analogReadDone = Ads.Read("Quote", analogReadValue) ' Analog read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> Quote: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), analogReadValue)

            analogReadDone = Ads.Read("Speed", analogReadValue) ' Analog read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> Speed: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), analogReadValue)

            analogReadDone = Ads.Read("BOARD_TEMP", analogReadValue) ' Analog read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> BOARD_TEMP: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), analogReadValue)

            analogReadDone = Ads.Read("BEARINGS_TEMP", analogReadValue) ' Analog read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> BEARINGS_TEMP: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), analogReadValue)

            analogReadDone = Ads.Read("STATOR_TEMP", analogReadValue) ' Analog read - the return value is the succeeded status. The read value is inside the variable
            Console.WriteLine("{0}{1} >> STATOR_TEMP: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), analogReadValue)

            analogReadDone = Ads.Read("P75_FW_VERSION", analogReadValue) ' Analog read - the return value is the succeeded status. The read value is inside the variable

            '' P75 version conversion
            Dim cent As Long
            Dim dec As Long
            Dim num As Long
            cent = analogReadValue And &HFF
            dec = (analogReadValue And &HFF00) / 256
            num = (analogReadValue And &HFF0000) / (256 * 256)

            'converto da carattere a num
            cent = cent - &H30
            dec = dec - &H30
            num = num - &H30

            analogReadValue = cent * 100 + dec * 10 + num
            Console.WriteLine("{0}{1} >> P75_FW_VERSION: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), analogReadValue)

            analogReadDone = Ads.Read("C39_FW_VERSION", analogReadValue) ' Analog read - the return value is the succeeded status. The read value is inside the variable
            '' C39 version conversion
            Dim cent1 As Long
            Dim dec1 As Long
            Dim num1 As Long
            dec1 = analogReadValue And &HFF
            num1 = (analogReadValue And &HFF00) / 256

            'converto da carattere a num
            dec1 = dec1 - &H30
            num1 = num1 - &H30

            analogReadValue = dec1 * 10 + num1
            Console.WriteLine("{0}{1} >> C39_FW_VERSION: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), analogReadValue)

            Dim allAnalogs() As Double
            allAnalogs = Ads.ReadMeasures()

            Ads.ReadAll()
            Console.WriteLine("{0}{1} >> A total of {2} channels read", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), Ads.Bits.Length + Ads.Measures.Length)
            ' **************************ATTIVARE PER FARE RESET ORE ****************
            ' Dim analogWriteDone As Boolean

            ' analogWriteDone = Ads.Write("RESET_STAT", 0)
            ' Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> RESET_STAT: 0")
            ' Thread.Sleep(1000)

            ' analogWriteDone = Ads.Write("RESET_STAT", 1)
            'Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> RESET_STAT: 1")
            ' Thread.Sleep(1000)

            ' analogWriteDone = Ads.Write("RESET_STAT", 0)
            ' Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> RESET_STAT: 0")
            ' ************************FINE RESET ORE *********************************

        Catch ex As Exception
            Console.WriteLine("{0}{1} >> Error occurred: {2}{3}{4}>> {5}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), Environment.NewLine, vbTab, vbTab, ex.Message)
        End Try

        Console.WriteLine("{0}{1} >> End{2}", Environment.NewLine, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), Environment.NewLine)

        ' End of communication
        Dim stopResult As Boolean = Ads.Stop()
        Console.WriteLine("{0}{1} >> Stop result: {2}", vbTab, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), stopResult)

        Console.WriteLine("------------------------------------------------------------")
        Console.Write("Press 'ENTER' to exit the application: ")
        Console.ReadLine()
    End Sub
End Module
