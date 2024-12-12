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
    )

    form.Controls.Add(proceedButton)
    form.Controls.Add(nameTextBox)
    form.Controls.Add(nameLabel)

    form