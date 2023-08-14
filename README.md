# Funda Makelaar Report

This is a quick Web API with a very basic angular frontend for querying the top 10 makelaars. The backend offers two endpoints: one endpoint provides the standard makelaar report (top 10 makelaars in amsterdam + top 10 makelaars in amsterdam offering garden properties); the other endpoint will return the top 10 makelaars for any provided query (no query validation currently provided).

Running the backend alone will startup SwaggerUI, allowing you to test the endpoints and see the JSON data for the tables. If wanting to see the data laid out in actual rows, you can start up the frontend by:
- making sure you have node.js installed (version ^16.14.0 || ^18.10.0)
- making sure you have the angular CLI installed
  - in terminal, run `npm install -g @angular/cli@16
- navigating to Frontend/funda-report-app in terminal and running `npm install`
- once that's done, run `ng serve` in the same terminal

Once the frontend is loaded, you should be able to navigate to localhost:4200 in your browser, which will hit the "standard makelaar" endpoint and display the requested information in two tables.
