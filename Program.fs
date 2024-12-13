open System
open System.Windows.Forms
open System.Drawing
open System.IO

let ticketFolder = Path.Combine("C:", "Users", "pc", "Documents", "CinemaBooking")

Directory.CreateDirectory(ticketFolder) |> ignore

// Initialize seating arrays for the two movies
let seatingCharts =
    [| "Movie1", Array2D.init 5 5 (fun   -> "Available")
       "Movie2", Array2D.init 5 5 (fun   -> "Available") |]

 // Main Form the Prompts the User to Select the movie they want to book.      
let rec createMainForm (customerName:string) =
    let form = new Form(Text = "Cinema Seat Booking System", Size = Size(400, 400))

    let customerNameLabel = new Label(
        Text = sprintf "Hello %s, What Movie would you like to book today?" customerName,
        Dock = DockStyle.Top, 
        TextAlign = ContentAlignment.MiddleCenter,  
        Height = 75  
    )

    let movie1Button = new Button(
        Text = "Movie 1", 
        Dock = DockStyle.Left, 
        Height = 40, 
        Width = 100
    )

    let movie2Button = new Button(
        Text = "Movie 2", 
        Dock = DockStyle.Right, 
        Height = 40, 
        Width = 100
    )

    form.Controls.Add(customerNameLabel)
    form.Controls.Add(movie2Button)
    form.Controls.Add(movie1Button)
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