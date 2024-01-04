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
    </head>
    <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4;'>
        <table cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;'>
            <tr>
                <td style='padding: 20px 0;'>
                    <center style='width: 100%;'>
                        <table cellpadding='0' cellspacing='0' width='60%' style='background-color: #fff; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); margin: auto; text-align: center;'>
                            <tr>
                                <td style='padding: 20px 0;'>
                                    <h1 style='color: #e74c3c;'>Welcome to Red Sparrow!</h1>
                                    <p style='color: #333;'>Dear " + userName + @",</p>
                                    <p style='color: #333;'>Thank you for registering with Red Sparrow. We are excited to have you on board!</p>
                                    <p style='color: #333;'>Our platform offers a wide range of services designed to <strong>enhance your experience</strong>.</p>
                                    <p style='color: #333;'>If you have any questions or need assistance, feel free to contact us at <a href='mailto:RedSparrow@gmail.com' style='color: #e74c3c;'>RedSparrow@gmail.com</a>.</p>
                                    <p style='margin-top: 20px;'><a href='http://192.168.10.102:81/login' style='display: inline-block; padding: 10px 20px; background-color: #e74c3c; color: #fff; text-decoration: none; border-radius: 5px;'>Get Started</a></p>
                                    <p style='color: #333; margin-top: 20px;'>Best regards,<br>Red Sparrow Team</p>
                                </td>
                            </tr>
                        </table>
                    </center>
                </td>
            </tr>
        </table>
    </body>
    </html>";
        }


        public static string GetFlightTickets(string email, List<ConnectionFlightTicketDto> tickets)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.Append(@"<!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <title>Flight Tickets - Red Sparrow</title>
        <style>
            /* General Styles */
            body {
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f4f4f4;
            }
            .container {
                width: 90%;
                max-width: 600px;
                margin: auto;
                text-align: center;
                background-color: rgba(255, 255, 255, 0.8);
                padding: 40px;
                border-radius: 10px;
                box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
                overflow-x: auto; /* Enable horizontal scrolling for content overflow */
            }
            h2 {
                color: #e74c3c;
                margin-bottom: 20px;
            }
            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
                overflow-x: auto; /* Enable horizontal scrolling for table overflow */
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
            .airline-message {
                margin-top: 20px;
                font-size: 18px;
                color: #333;
            }
        </style>
    </head>
    <body>
        <div style='background-image: url(""https://img.freepik.com/premium-photo/red-table-with-toy-plane_1921-1781.jpg?size=626&ext=jpg&ga=GA1.1.1546980028.1702598400&semt=ais""); background-repeat: no-repeat; background-size: cover;'>
            <div class='container'>
                <h2>Flight Tickets - Red Sparrow</h2>
                <p>Email sent to: " + email + @"</p>
                <div style='overflow-x: auto;'> <!-- Surround the table with a div for scrolling -->
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

            // Adding a personalized message from the airline
            htmlBuilder.Append(@"</table>
                    </div>
                    <p class='airline-message'>Dear Passenger, <br>Thank you for choosing Red Sparrow Airlines. Wishing you a pleasant journey filled with wonderful experiences. Have a safe and enjoyable flight!</p>
                </div>
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
                .cancellation-message {
                    color: #e74c3c;
                    font-size: 18px;
                    margin-top: 20px;
                }
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Flight Tickets - Red Sparrow</h2>
                <p>Email sent to: " + email + @"</p>
                <p class='cancellation-message'>Dear Passenger, <br>Your ticket(s) have been canceled.</p>
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
.cancellation-message {
                    color: #e74c3c;
                    font-size: 18px;
                    margin-top: 20px;
                }
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Flight Tickets - Red Sparrow</h2>
            <p class='cancellation-message'>Dear Passenger, <br>Your Booking has been canceled.</p>
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
