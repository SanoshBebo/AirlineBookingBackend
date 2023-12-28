using SanoshAirlines.Controllers;
using System.Text;

namespace SanoshAirlines.Models
{
    public class EmailTemplates
    {
        
        public static string GetWelcomeEmailBody(string userName)
        {
            return @"<!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <title>Welcome to Red Sparrow!</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }
                    .container {
                        width: 60%;
                        margin: auto;
                        text-align: center;
                        background-color: #fff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }
                    h1 {
                        color: #e74c3c;
                    }
                    p {
                        color: #333;
                    }
                    .cta-button {
                        display: inline-block;
                        margin-top: 20px;
                        padding: 10px 20px;
                        background-color: #e74c3c;
                        color: #fff;
                        text-decoration: none;
                        border-radius: 5px;
                    }
                    .cta-button:hover {
                        background-color: #c0392b;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Welcome to Red Sparrow!</h1>
                    <p>Dear " + userName + @",</p>
                    <p>Thank you for registering with Red Sparrow. We are excited to have you on board!</p>
                    <p>Our platform offers a wide range of services designed to <strong>enhance your experience</strong>.</p>
                    <p>If you have any questions or need assistance, feel free to contact us at RedSparrow@gmail.com .</p>
                    <a href='http://192.168.10.102:81/login' class='cta-button'>Get Started</a>
                    <p>Best regards,<br>Red Sparrow Team</p>
                </div>
            </body>
            </html>";
        }

        public static string GetFlightTickets(string email, List<ConnectionFlightTicketDto> tickets)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            // HTML styling for the email body
            htmlBuilder.Append(@"<!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <title>Flight Tickets - Red Sparrow</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f4f4f4;
            }
            .container {
                width: 60%;
                margin: auto;
                text-align: center;
                background-color: #fff;
                padding: 20px;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }
            h2 {
                color: #e74c3c;
            }
            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }
            th, td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }
            th {
                background-color: #e74c3c;
                color: white;
            }
            .ticket-info {
                color: #333;
                font-size: 16px;
            }
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Flight Tickets - Red Sparrow</h2>
            <p>Email sent to: " + email + @"</p>
            <table>
                <tr>
                    <th>Ticket No</th>
                    <th>Booking ID</th>
                    <th>Flight Name</th>
                    <th>Source Airport</th>
                    <th>Destination Airport</th>
                    <th>Seat No</th>
                    <th>Name</th>
                    <th>Age</th>
                    <th>Gender</th>
                    <th>Date & Time</th>
                </tr>");

            // Adding ticket details to the table
            foreach (var ticket in tickets)
            {
                htmlBuilder.Append("<tr class='ticket-info'>");
                htmlBuilder.Append($"<td>{ticket.TicketNo}</td>");
                htmlBuilder.Append($"<td>{ticket.BookingId}</td>");
                htmlBuilder.Append($"<td>{ticket.FlightName}</td>");
                htmlBuilder.Append($"<td>{ticket.SourceAirportId ?? "N/A"}</td>");
                htmlBuilder.Append($"<td>{ticket.DestinationAirportId ?? "N/A"}</td>");
                htmlBuilder.Append($"<td>{ticket.SeatNo}</td>");
                htmlBuilder.Append($"<td>{ticket.Name}</td>");
                htmlBuilder.Append($"<td>{ticket.Age}</td>");
                htmlBuilder.Append($"<td>{ticket.Gender}</td>");
                htmlBuilder.Append($"<td>{ticket.DateTime}</td>");
                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append(@"</table>
        </div>
    </body>
    </html>");

            return htmlBuilder.ToString();
        }


        public static string CancelFlightTickets(string email, List<ConnectionFlightTicketDto> tickets)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            // HTML styling for the email body
            htmlBuilder.Append(@"<!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <title>Flight Tickets - Red Sparrow</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f4f4f4;
            }
            .container {
                width: 60%;
                margin: auto;
                text-align: center;
                background-color: #fff;
                padding: 20px;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }
            h2 {
                color: #e74c3c;
            }
            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }
            th, td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }
            th {
                background-color: #e74c3c;
                color: white;
            }
            .ticket-info {
                color: #333;
                font-size: 16px;
            }
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Flight Tickets - Red Sparrow</h2>
            <p>Email sent to: " + email + @"</p>
            <table>
                <tr>
                    <th>Ticket No</th>
                    <th>Booking ID</th>
                    <th>Flight Name</th>
                    <th>Source Airport</th>
                    <th>Destination Airport</th>
                    <th>Seat No</th>
                    <th>Name</th>
                    <th>Age</th>
                    <th>Gender</th>
                    <th>Date & Time</th>
                </tr>");

            // Adding ticket details to the table
            foreach (var ticket in tickets)
            {
                htmlBuilder.Append("<tr class='ticket-info'>");
                htmlBuilder.Append($"<td>{ticket.TicketNo}</td>");
                htmlBuilder.Append($"<td>{ticket.BookingId}</td>");
                htmlBuilder.Append($"<td>{ticket.FlightName}</td>");
                htmlBuilder.Append($"<td>{ticket.SourceAirportId ?? "N/A"}</td>");
                htmlBuilder.Append($"<td>{ticket.DestinationAirportId ?? "N/A"}</td>");
                htmlBuilder.Append($"<td>{ticket.SeatNo}</td>");
                htmlBuilder.Append($"<td>{ticket.Name}</td>");
                htmlBuilder.Append($"<td>{ticket.Age}</td>");
                htmlBuilder.Append($"<td>{ticket.Gender}</td>");
                htmlBuilder.Append($"<td>{ticket.DateTime}</td>");
                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append(@"</table>
        </div>
    </body>
    </html>");

            return htmlBuilder.ToString();
        }

        public static string CancelBooking(string email, List<ConnectionFlightTicketDto> tickets)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            // HTML styling for the email body
            htmlBuilder.Append(@"<!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <title>Flight Tickets - Red Sparrow</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f4f4f4;
            }
            .container {
                width: 60%;
                margin: auto;
                text-align: center;
                background-color: #fff;
                padding: 20px;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }
            h2 {
                color: #e74c3c;
            }
            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }
            th, td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }
            th {
                background-color: #e74c3c;
                color: white;
            }
            .ticket-info {
                color: #333;
                font-size: 16px;
            }
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Flight Tickets - Red Sparrow</h2>
            <p>Email sent to: " + email + @"</p>
            <table>
                <tr>
                    <th>Ticket No</th>
                    <th>Booking ID</th>
                    <th>Flight Name</th>
                    <th>Source Airport</th>
                    <th>Destination Airport</th>
                    <th>Seat No</th>
                    <th>Name</th>
                    <th>Age</th>
                    <th>Gender</th>
                    <th>Date & Time</th>
                </tr>");

            // Adding ticket details to the table
            foreach (var ticket in tickets)
            {
                htmlBuilder.Append("<tr class='ticket-info'>");
                htmlBuilder.Append($"<td>{ticket.TicketNo}</td>");
                htmlBuilder.Append($"<td>{ticket.BookingId}</td>");
                htmlBuilder.Append($"<td>{ticket.FlightName}</td>");
                htmlBuilder.Append($"<td>{ticket.SourceAirportId ?? "N/A"}</td>");
                htmlBuilder.Append($"<td>{ticket.DestinationAirportId ?? "N/A"}</td>");
                htmlBuilder.Append($"<td>{ticket.SeatNo}</td>");
                htmlBuilder.Append($"<td>{ticket.Name}</td>");
                htmlBuilder.Append($"<td>{ticket.Age}</td>");
                htmlBuilder.Append($"<td>{ticket.Gender}</td>");
                htmlBuilder.Append($"<td>{ticket.DateTime}</td>");
                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append(@"</table>
        </div>
    </body>
    </html>");

            return htmlBuilder.ToString();
        }

    }
}
