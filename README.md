# Godot C# Runtime Inspector

## Introduction
This is a Godot addon (4.3 and later) for opening an inspector for c# classes at runtime. It implements a frontend for the [LgkProductions Inspector](https://github.com/LGK-Productions/LgkProductions.Inspector). The inspector can be customized using various tags which are resolved by the 

## Installation
Put the addons/settings_inspector folder into the addons folder of your godot project. Drag the MemberInspectorHandler.tscn scene into the scene you want to open inspectors in.

## API
A new Inspector can be opened using:
``` c#
private TestModel testObject = new();
var handle = MemberInspectorHandler.Instance.OpenClassInspectorWindow(testObject);
```
This will open a new native window containing an inspector for the testObject. The handle contains an OnApply and OnClose event, being called if apply/close are pressed.
Apply writes the settings back into the test object, Close closes the inspector.

## Tags
The inspector can be customized by adding c# tags to members/classes. You can find available tags in the following list:

### General
|Name   |Parameters   |Targets   |Effect   |
|---|---|---|---|
|**ShowInInspector**|-|Field/Property|Shows the member in the inspector|
|**HideInInspector**|-|Field/Property|Hides the member from the inspector|
|**DisplayName**|(string) name|Field/Property|Alters the displayed name of the member|
|**ReadOnly**|-|Field/Property|Sets the member to be read only (if not already)|
|**Serializable**|-|Class|Sets the class to be serializable, enabling save/load buttons being shown in the inspector|
|**Description**|(string) description|Field/Property|Adds a description to the member, being displayed when hovered|

### Layout
|Name   |Parameters   |Targets   |Effect   |
|---|---|---|---|
|PropertyOrder|(int) order|Field/Property|Sets the given order (higher means further down)|
