Imports EthercatByAds

''' <summary>
''' Ads DLL example application
''' </summary>
Module Program
    ''' <summary>
    ''' Application entry-point
    ''' </summary>
    Sub Main()
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Start{Environment.NewLine}")

        Try
            '' ******** Initialization of the Ads communication - local amsnet id ********
            Dim initResult As Boolean = Ads.Initialize("169.254.174.61.1.1", 851).Result
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Initialization result: {initResult}")
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Communication in error: {Ads.IsInError}")
            If Ads.IsInError Then
                Console.WriteLine($"{vbTab}{vbTab}>> Reason of failure: {Ads.ReasonOfFailure}")
            End If

            '' ******** Example of write operations - method overloads ********
            Dim digitalWriteResult As Boolean = Ads.Write("FirstDigitalOutput", True) '' Digital write - the return value is the succeeded status
            digitalWriteResult = digitalWriteResult And Ads.Write("SecondDigitalOutput", True)
            Console.WriteLine($"{vbTab}{vbTab}>> Digitals write result: {digitalWriteResult}")

            Dim analogWriteResult As Boolean = Ads.Write("FirstAnalogOutput", Math.E) '' Analog write - the return value is the succeeded status
            analogWriteResult = analogWriteResult And Ads.Write("SecondAnalogOutput", Math.PI)
            Console.WriteLine($"{vbTab}{vbTab}>> Analogs write result: {analogWriteResult}")

            '' ******** Example of read operations - ByRef variables ********
            Dim firstDigitalReadValue, secondDigitalReadValue As Boolean
            Dim firstAnalogReadValue, secondAnalogReadValue As Double
            Dim digitalReadResult As Boolean = Ads.Read("FirstDigitalInput", firstDigitalReadValue) '' Digital read - the return value is the succeeded status
            digitalReadResult = digitalReadResult And Ads.Read("SecondDigitalInput", secondDigitalReadValue)

            Dim analogReadResult As Boolean = Ads.Read("FirstAnalogInput", firstAnalogReadValue) '' Analog read - the return value is the succeeded status
            analogReadResult = analogReadResult And Ads.Read("SecondAnalogInput", secondAnalogReadValue)

            '' Value read from the PLC
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Digitals read value: {firstDigitalReadValue}; {secondDigitalReadValue}")
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Analogs read value: {firstAnalogReadValue}; {secondAnalogReadValue}")
        Catch ex As Exception
            Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Error occurred: {Environment.NewLine}{ex}")
        End Try

        Console.WriteLine($"{Environment.NewLine}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> End{Environment.NewLine}")

        '' ******** Communication stop ********
        Dim stopResult As Boolean = Ads.Stop()
        Console.WriteLine($"{vbTab}{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} >> Stop result: {stopResult}")

        Console.WriteLine("------------------------------------------------------------")
        Console.Write("Press 'ENTER' to exit the application: ")
        Console.ReadLine()
    End Sub
End Module
