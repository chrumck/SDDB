##SDDB

###General Description
**SDDB** is a database with a web user interface which purpose is to help track progress of the company's projects, track hardware usage belonging to the company, as well as  record people activities and training progress.

Once you log in, you can jump straight to **Your Activity** and start entering your hours and activities, but read on if you want to know more about how the information is stored in SDDB.

The main logical structure of the information contained in SDDB starts with **Projects**. SDDB Projects usually reflect the actual projects executed by the company, but can also represent company divisions or accounts like General & Administrative or Stock.
Projects are divided into smaller parts called **Locations**. Locations most often represent geographical areas of a Project (tunnel, excavation, building etc.), but can also split the Project into other logical parts (for example server or office).
Locations are in turn collections of **Assemblies**. Assemblies are subsystems or installations which usually represent instruments or groups of instruments installed on a project. An Assembly can be also a MULTIROLE system comprising of a control box collecting data from several types of instruments at once.
Assemblies are also collections of **Components**. Components represent any piece of hardware which is a part of an installation or a subsystem and has one or more of the following attributes: it is reusable and can be moved between Assemblies (including changing Projects); is valuable enough to be tracked in SDDB and maintained by the company; is uniquely distinguishable by a serial number or other tag; needs to be calibrated on regular basis.

###Demo Version
A demo copy of SDDB is available [here on Azure](https://sddb.azurewebsites.net/) . You can use guest / gu3st credentials to log in.
