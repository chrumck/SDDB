##SDDB Help Files

###General Description
**SDDB** is a database with a web user interface which purpose is to help track progress of the company's projects, track hardware usage belonging to the company, as well as  record people activities and training progress.

The main logical structure of the information contained in SDDB starts with **Projects**. SDDB Projects usually reflect the actual projects executed by the company, but can also represent company divisions or accounts like General & Administrative or Stock.
Projects are divided into smaller parts called **Locations**. Locations most often represent geographical areas of a Project (tunnel, excavation, building etc.), but can also split the Project into other logical parts (for example server or office).
Locations are in turn collections of **Assemblies**. Assemblies are monitoring subsystems or installations which usually represent instruments or groups of instruments. The examples of an Assembly is an AMTS installation (a Total Station with a control box and a power supply) or an MPBX Assembly (with an MPBX instrument, a datalogger and a battery). An Assembly can also be a MULTIROLE system comprising of a control box collecting data from several types of instruments at once like Crack Gages, Tilt Meters, Piezometer, In-Place Inclinometers.
Assemblies are also collections of **Components**. Components represent any piece of hardware which is a part of an installation or a subsystem mentioned above and has one or more of the following attributes: it is reusable and can be moved between Assemblies (including changing Projects); is valuable enough to be tracked and maintained; is uniquely distinguishable by a serial number or other tag; needs to be calibrated on regular basis.

When setting up a new project in SDDB, it is worth remembering that a balance is needed on how many Locations, Assemblies and Components are created for each Project. On one hand, the division has to be granular enough so the logical divisions do not become too vague. On the other hand, if the division is too detailed, the structure is very often redundant. Remember that the structure of Project -> Location -> Assembly has to primarily meet two goals: Firstly, it has to be relatively easy to memorize and allow quick navigation down the tree of the division. Secondly, all staff **Activities** which are recorded in SDDB have to be assigned to a Location and an Assembly. The assignment itself should be clear enough so little additional information or comment is needed in order to define the given activity in sufficient detail.
A sign of a structure which may be not optimal is when there is only one element in a given group (for example one Assembly in a Location). It may be in this case useful to merge such group with another one. Conversely, if there are hundreds or thousands of elements in a group (for example Components in an Assembly), it may be beneficial to split the group into smaller parts.
