# BlogFull
An ASP.NET 4.7 MVC markdown blog framework built for speed and getting things right. Fully functional with the minimum required features to make a blog _nice_, while following best practices along the way.

## Features
* Fast as hell, first paint in 300ms-350ms (locally) or 350ms-450ms (hosted) range.
* Scores 100 for Performance on Google audit (desktop and mobile).
* Scores 100 for Accessibility on Google audit (desktop and mobile).
* Scores 100 for Best Practices on Google audit (desktop and mobile).
* Scores 100 for SEO on Google audit (desktop and mobile).
* Built in Disqus comment/response support.
* Built in reCAPTCHA support.
* Built in drag-drop image upload and embed.
* Edit and post in markdown, render as html.
* No database, uses flat file xml storage.

Proof:

![Built for speed](https://webproject.za.net/Certifications/BlogFull_Report.JPG "Sample Hosted Google Audit Report")

# Instructions

## Install Instructions
1. Clone repo.
2. Open with Visual Studio, edit web.config variables.
3. Place whatever content you want in the /Home view, or delete the controller for no Home page.
4. Build and publish to your website.
5. Send me the link on [Twitter](https://twitter.com/jacob_pretorius "Jacob's Twitter") so I can share it.


## Usage Instructions
1. Navigate to /admin and login with the username and password you set.
2. Start blogging.
2.1. Click on an uploaded image to copy the markdown link to clipboard.

# With Thanks
* Initial base (sort of, mostly the xml storing and parsing): [MiniBlog](https://github.com/madskristensen/MiniBlog).
* Markdown parsing: [CommonMark.NET](https://github.com/Knagis/CommonMark.NET).
* Drag drop image upload in pure JS based on the work by [Hoe Zim](https://codepen.io/joezimjs/pen/yPWQbd).
* Frontent Beautification: Jean-Paul "JP" Kleynhans.
