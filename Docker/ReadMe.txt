With the files in the Docker folder it is possible to create Docker
images for DyVisual which can be run in a Linux or Windows
environment.

For now two Docker images can be generated:

generic: A generic DyVisual image which can be used for the
	 development of own applications.

Whirlpool: An example application for the visualization of deviation
	   maps. This example was created in the context of the FITMAN
	   project (http://www.fitman-fi.eu/).

The Makefile works only in a Unix/Linux environment. However, for
users not really familiar with Docker it might be still helpful to see
which commands could be used in a Windows environment.

To create the images use:

make create DF=Whirlpool/DyVisual-WP

or

make create DF=generic/DyVisual

To run the created images use:

make run IMAGE=<image>

or

make rund IMAGE=<image>

where the run starts the Docker image in interactive mode, to make
interaction with the FiVES server possible. rund starts the Docker
image as a deamon without tty connection.

To delete an image use:

make remove IMAGE=<image>

In case the image was started as a deamon use

make remove-c IMAGE=<container-id>

to delete the container.

In these commands <image> needs to be replaced by the IMAGE ID. IMAGE
ID is displayed in the last line of the creation output or can be
displayed by the Docker command "docker images".

When the Docker image is run it should be possible to connect to
DyVisual with Google Chrome or Mozilla Firefox using the URL:

http://localhost/WebClient/client.xhtml
or
http://localhost/WebClientWp/client.html

respectively. If port 80 is already busy on the host machine the port
mapping needs to be changed in the "docker run" command.

In the Whirlpool example the deviation map can be zoomed by moving the
mouse while the right mouse button is pressed and rotated with pressed
left mouse button.
