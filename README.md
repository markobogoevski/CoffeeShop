
<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Getting Started](#getting-started)
  * [Installation](#installation)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)



<!-- ABOUT THE PROJECT -->
## About The Project

We made this project because of our great love of coffee. It is a simple yet quite functional website. We meet the needs of even the most demanding of customers. We hope to go beyond your expectations.
We have a menu which from you can order lots of different coffees and even create your own custom coffees.
There are plenty of ingredients to choose from, and the coffee shop owners or administrators can update the list as they please.
 
Top features:
* A myriad of ingredients to choose from. Coffee is something very personal and part of our daily routine. So make it your own! 
* Not a big fan of variety. Don't worry, we got you. We already have built some coffees that are waiting to be ordered.
*  Our coffee is made from real baristas and we have timely delivery so you can enjoy your coffee hot or ice cold.
* Full management and control of orders.
* Useful filters and thought-out ways to  ease the experience of the customer.
* A personal shopping cart with the ability to postpone orders for later.

Are you a manager? Then our site offers:
* Real time view and management of all the orders that are placed by the users.
* Real time view and management of all the user custom made coffee, as well as full control over the shop's coffees and ingredients.
* Add new coffees and ingredients with just a few clicks.
* Full statistics for each coffee and ingredient including calculation of total and weekly profit, as well as calculation of number of times an item has been sold in total or per week.
* Stock management of each coffee and ingredient.

Our web app is also responsive and can be used on all platforms.

### Built With
* [Bootstrap](https://getbootstrap.com)
* [ASP.NET MVC](https://dotnet.microsoft.com/apps/aspnet/mvc)

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Installation
1. Clone the repo
```sh
git clone https://github.com/markobogoevski/CoffeeShop
```
2. Run update-database in the NuGet package manager
3.  Start the project
* The project has a seed method which initializes a starting database.
*** Important. If any problems arise with the .mdf file -> Delete database, delete the .mdf file, delete every migration in the Migrations folder EXCEPT the Configuration.cs file, remove the connection from the server view explorer. Then enable-migrations from the nuget package manager console; create an initial migration
with create migration "migrationName" then update database.

<!-- USAGE EXAMPLES -->
## Usage
* Our navbar changes in accordance to the authorization level of the user. The footer consists of social media links and contacts. It also shows the location of the nearest coffee shop. 
* On the homepage we have a carousel which automatically lists our top most sold coffees. 
* There are ingredient and coffee views which can be viewed by everyone even unauthorized. 
* In the datatables you can toggle the images if there are some
* Accounts: 
 Administrator role: Email: admin@test.com; 
 Owner role: Email: owner@test.com
 User role: Email: user@test.com
 All share the same password: Password1!

### Up next are some role specific features

#### Unauthorized
An unauthorized user has access to the Home page of the web site alongside the Index Views for the Coffee and Ingredient controllers. The user can look through all of the shop-made coffees and ingredients but isn't able to edit, delete, create nor place an order. The user can also filter the coffee by its ingredients. Only the ingredients used in the coffees which are for sale are shown in the filtering list. 

#### User
The user has all the privileges an unauthorized user has. As extra, the user can create his own custom coffee by selecting from a list which offers all the ingredients the shop has currently in stock. If an ingredient is not in stock, it will not be prompted as an option to the user when making the coffee. The user can choose to buy a coffee that has been custom made by him or which has been made by the shop (any admin or owner). Upon buying, he is prompted to choose a coffee size according to which the price of the coffee is updated. The maximum quantity of a coffee the user can buy depends on the quantity in stock of that coffee. The user can also choose to view the coffee of the day, which is prompted with 30% discount. The user doesn't directly buy the item, but rather adds it to his shopping cart. There, the user can further manage the quantity of the item he bought, either increasing or decreasing it. He also has an option to remove an item from the cart and clear out the whole cart if he wants. After entering an address, the user creates the order. Upon creating the order, the stock of coffee and ingredient items which were used in the order items are updated. In the orders view he can see all of his orders. There he can filter the orders according to their status:  pending, finished, inactive or cancelled. The user can at anytime view the details of the order, where each order item is explained alongside its price. Upon creating an order, its status is automatically set to inactive. It is the user's responsibility to change the status of the order from inactive to pending whenever he feels like he wants to order. He can also remove an inactive order, deleting it from the database. A user can always cancel his order if the order is not in the 5 next orders to be finished by the staff. Cancelling an order changes the order status from pending to inactive. After the order has been finished (set by any admin or owner), the user can choose to discard the order from his orders view. This doesn't remove the order from database enabling managers to view a full history of each finished order. The user can also rate a finished order. 

#### Administrator and Owner 
For brevity I will use admin for both Administrator and Owner. They have the same authorization level and privileges. That can be changed in the future if necessary. 
The admin can access three views from the navbar. Those are Ingredients, Coffee and Orders. In the coffee view he can edit and delete whatever he pleases. He can achieve that by clicking on some of the icons in the datatable rows at the end. The admin can create a coffee for the shop which later is shown to every user on the site. There are statistics provided for the coffee and for the ingredients accordingly. In the statistics for the coffees he can view the most sold and least sold from all time or weekly. Upon clicking on some of those button he is taken to a page that displays the coffee details together with the statistics that accompany it. There is also a datatable provided with statistics for every single coffee that is created. In this view the admin can also increase and decrease the stock quantity. For the ingredients the same holds as the coffee with some subtleties. You cannot view the ingredient of the day since there is no such feature. In the according statistics he can view the most and least used ingredients weekly and from all time. Everything else is the same as from the coffee view. Orders is the last view that is relevant. There the admin can filter the orders same as the user can. There is also a button which removes all inactive orders. All of the orders are shown there and he can remove, complete or view the details. The admin can force-cancel any pending order rendering its status to cancelled. A cancelled order cannot be made pending again. The admin can also remove any order he wants, which removes it from database. Most importantly, the admin manages the activation of an inactive order made by a user, notifying the user that the order is on the way.

<!-- CONTACT -->
## Contact

Marko Bogoevski -  markobogoevski@gmail.com
David Galevski - davidgalevski@gmail.com
Mladen Tasevski - mladen.tasevski@gmail.com

<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [Bootbox](http://bootboxjs.com/) 
* [jquery DataTables](https://datatables.net/)
* [Font Awesome](https://fontawesome.com)
