open System
open System.Windows.Forms
open System.Drawing
open System.IO

let ticketFolder = Path.Combine("C:", "Users", "pc", "Documents", "CinemaBooking")

Directory.CreateDirectory(ticketFolder) |> ignore

// Initialize seating arrays for the two movies
let seatingCharts =
    [| "Movie1", Array2D.init 5 5 (fun _ _   -> "Available")
       "Movie2", Array2D.init 5 5 (fun _ _   -> "Available") |]
       
// Load existing seat data from files if available
let loadSeatingChart (movieName:string) =
    let filePath = Path.Combine(ticketFolder, movieName + ".txt")
    if File.Exists(filePath) then
        let lines = File.ReadAllLines(filePath)
        let chart = Array2D.init 5 5 (fun _ _ -> "Available")
        lines |> Array.iter (fun line ->
            let parts = line.Split(',')
            let row, col = int parts.[0], int parts.[1]
            chart.[row, col] <- "Reserved"
        )
        chart
    else
        Array2D.init 5 5 (fun _ _ -> "Available")

//Saving Reserved Seats to a File
let saveSeatingChart (movieName: string) (chart: string[,]) =
    let filePath = Path.Combine(ticketFolder, movieName + ".txt")
    let reservedSeats =
        [| for row in 0 .. 4 do
               for col in 0 .. 4 do
                   if chart.[row, col] = "Reserved" then
                       yield sprintf "%d,%d" row col |]

    File.WriteAllLines(filePath, reservedSeats)
    
//Marking the Seat as Reserved to prevent Double-Booking of the same seat
let reserveSeat (movieName: string) (chart: string[,]) (row: int) (col: int) (customerName: string) (movieShowtime: string) : Option<string> =
    if chart.[row, col] = "Available" then
        chart.[row, col] <- "Reserved"
        saveSeatingChart movieName chart
        //Generating Ticket ID
        let ticketID = movieName+"-"+ row.ToString()+"/"+ col.ToString()+"-"+ customerName  
        //Generating a Unique Ticket Details and Saving it to a file
        let ticketDetails = sprintf "Ticket ID: %s\nCustomer: %s\nMovie: %s\nShowtime: %s\nSeat: Row %d, Col %d\n" ticketID customerName movieName movieShowtime (row + 1) (col + 1)  
        File.AppendAllText(Path.Combine(ticketFolder, movieName + "_tickets.txt"), ticketDetails + "\n")  
        Some ticketID
    else
        None

 // Main Form the Prompts the User to Select the movie they want to book.      
let rec createMainForm (customerName:string) =
    let form = new Form(Text = "Cinema Seat Booking System", Size = Size(400, 400))

    let movieShowtimes = [| "Movie1", "8:00 PM -  Monday"; "Movie2", "10:00 PM - Sunday" |]
    let firstMovieShowtime = snd movieShowtimes.[0]
    let secondMovieShowtime = snd movieShowtimes.[1]

    let customerNameLabel = new Label(
        Text = sprintf "Hello %s, What Movie would you like to book today?" customerName,
        Dock = DockStyle.Top, 
        TextAlign = ContentAlignment.MiddleCenter,  
        Height = 75  
    )

    let movie1Button = new Button(
        Text = sprintf "Movie 1\n Movie Showtime: %s" firstMovieShowtime, 
        Dock = DockStyle.Fill, 
        Height = 40, 
        Width = 100
    )

    let movie2Button = new Button(
        Text = sprintf "Movie 2\n Movie Showtime: %s" secondMovieShowtime, 
        Dock = DockStyle.Bottom, 
        Height = 150
    )

    movie1Button.Click.Add(fun _ -> form.Hide(); createMovieForm "Movie1" customerName firstMovieShowtime)
    movie2Button.Click.Add(fun _ -> form.Hide(); createMovieForm "Movie2" customerName secondMovieShowtime)
    form.Controls.Add(customerNameLabel)
    form.Controls.Add(movie2Button)
    form.Controls.Add(movie1Button)
    form.Show()

// Movie Form
and createMovieForm (movieName:string) (customerName:string) (movieShowtime: string) =
    let form = new Form(Text = sprintf "%s - Seat Selection" movieName, Size = Size(550, 350))
    let chart =
        match seatingCharts |> Array.tryFind (fun (name, _) -> name = movieName) with
        | Some (_, chart) -> chart
        | None -> failwith "Movie not found"
    let gridPanel = new TableLayoutPanel(RowCount = 5, ColumnCount = 5, Dock = DockStyle.Fill)
    let infoLabel = new Label(Text = "Select a seat", Dock = DockStyle.Top, AutoSize = true, TextAlign=ContentAlignment.MiddleCenter)
    //Displaying Available and Reserved Seats in GUI 
    let seatButtons =
        [| for row in 0 .. 4 do
               for col in 0 .. 4 do
                   let button = new Button(Text = sprintf "%d,%d" (row + 1) (col + 1), Dock = DockStyle.Fill, Height=50, Width=100)
                   button.BackColor <- if chart.[row, col] = "Reserved" then Color.Red else Color.LightGreen
                   button.Click.Add(fun _ ->
                       if chart.[row,col] = "Available" then
                        //Marking the Seat as Reserved to prevent Double-Booking of the same seat
                           let ticketID = reserveSeat movieName chart row col customerName movieShowtime //Allowing the User to Select the seat by column, row indicies (if available) 
                           //Generating a Unique TicketID
                           match ticketID with
                           | Some id ->
                               form.Hide()
                               createDetailsForm customerName movieName (row, col) id movieShowtime
                           | None -> MessageBox.Show("Unable to reserve seat.") |> ignore
                       else
                           MessageBox.Show("This seat is already reserved.") |> ignore)
                   yield button
        |]

    seatButtons |> Array.iter (fun button -> gridPanel.Controls.Add(button))

    form.Controls.Add(gridPanel)
    form.Controls.Add(infoLabel)
    form.Show()

// Details Form
and createDetailsForm (customerName:string) (movieName:string) (row, col) (ticketID:string) (movieShowtime:string) =
    let form = new Form(Text = "Booking Details", Size = Size(300, 300))
    let detailsLabel = new Label(Text = sprintf "Customer: %s\nMovie: %s\nShow Time: %s\nSeat: Row %d, Col %d\nTicket ID: %s" customerName movieName movieShowtime (row + 1) (col + 1) ticketID, Dock = DockStyle.Fill)
    let exitButton = new Button(Text = "Exit", Dock = DockStyle.Bottom, Width=145)

    exitButton.Click.Add(fun _ -> form.Close(); Application.Exit();)

    form.Controls.Add(detailsLabel)
    form.Controls.Add(exitButton)
    form.Show()

    
// Main Form (Prompts the User to Enter Their Name)
let createEntryForm () =
    let form = new Form(Text = "Enter Your Name", Size = Size(500, 300))
    let nameLabel = new Label(Text = "Enter your name:", Dock = DockStyle.Top)
    let nameTextBox = new TextBox(Dock = DockStyle.Top)
    let proceedButton = new Button(Text = "Proceed", Dock = DockStyle.Bottom)

    proceedButton.Click.Add(fun _ ->
        if String.IsNullOrWhiteSpace(nameTextBox.Text) then
            MessageBox.Show("Please enter your name.") |> ignore
        else
            form.Hide()
            let customerName = nameTextBox.Text
            createMainForm customerName
    )

    form.Controls.Add(proceedButton)
    form.Controls.Add(nameTextBox)
    form.Controls.Add(nameLabel)

    form

[<STAThread>]
do
    seatingCharts |> Array.iter (fun (name, chart) ->
        let loadedChart = loadSeatingChart name
        Array2D.blit loadedChart 0 0 chart 0 0 5 5)
    Application.Run(createEntryForm())
