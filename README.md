<p align="center">
  <img width="200" height="200" src="https://raw.githubusercontent.com/jamerst/OpenLD/master/res/openld_plain.svg">
</p>

<h1 align="center">OpenLD</h1>
<h3 align="center">A collaborative CAD System for creating lighting designs</h3>

OpenLD lets you create professional-looking lighting designs, straight from your browser. You can share and collaborate on your drawings with others using built-in real-time editing functionality.

## Features
- Collaborative editing
- Easy-to-use interface
- Export to PDF
- Fixture library
- Cross-platform
- Self-hostable
- Completely free!

## About
OpenLD was the subject of my dissertation, created as my final year project at the University of Warwick. Inspired by my work with the [technical theatre society](https://www.warwicktechcrew.co.uk/), I sought to create an alternative to existing CAD tools that was freely available, supported collaboration, and easier to use, in order to improve the experience of lighting designers in the society.

OpenLD is a web-application written using the ASP.NET Core back-end framework with a React.js front-end. Developing OpenLD increased my experience and confidence in many areas, including designing systems, unit testing, deployment, and project management. My experience and confidence with C#, ASP.NET Core and React.js was also significantly increased by working on a sufficiently complex project.

## Running OpenLD
**Note: A Chromium-based browser is recommended for performance reasons.**

_This project is not actively maintained, so the live demo is no longer available._

### Self Hosting

OpenLD is a dockerised application, so running it yourself is relatively easy. Simply clone the repository, and run `docker-compose up` in the `openld` directory to start the development build, which will be accessible at https://0.0.0.0:5000/.

Running the production build is slightly more complex due to requiring certificates for HTTPS and IdentityServer4. Generate the following certificates on the host machine to run the system:
```
/certs/openld-https.pfx
/certs/openld-is4-key.pfx
```
Generating these is relatively simple using openssl. Certificates should be password-protected, modify `docker-compose-prod.yml` to add the password for the HTTPS certificate, the password for the IS4 certificate is given in `appsettings.json`.

OpenLD seeds some initial data, which includes some common fixtures and venue templates for the Warwick Arts Centre. To prevent initial data seeding, delete `openld/init-data/initdb.d/20-data.sql`.

Start the production build by running `docker-compose -f docker-compose-prod.yml up` in the `openld` directory.

Tested under Linux, should work under other environments, but this hasn't been tested.
