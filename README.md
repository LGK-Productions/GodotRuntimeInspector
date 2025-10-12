# Godot C# Runtime Inspector

## Introduction
This is a Godot addon (4.5 and later) for opening an inspector for c# classes at runtime. It implements a frontend for the [LgkProductions Inspector](https://github.com/LGK-Productions/LgkProductions.Inspector). The inspector can be customized using various tags which are resolved by the Metadata backend.

## Setup
Put the addons/runtime_inspector folder into the addons folder of your godot project. Drag the MemberInspectorHandler.tscn scene into the scene you want to open inspectors in.

## API
Inspectors can be created in two different ways:
1. As a standalone floating window using:
``` c#
private TestModel testObject = new();
var handle = MemberInspectorHandler.Instance.OpenClassInspectorWindow(testObject);
```
This will open a new native window containing an inspector for the testObject. The handle contains an OnApply and OnClose event, being called if apply/close are pressed.
Apply writes the settings back into the test object, Close closes the inspector.

2. As a ui node which can be added manually into the scene tree via
``` c#
private TestModel testObject = new();
var wrapper = MemberInspectorHandler.Instance.ConstructMemberWrapper(testObject)
```
this returns a MemberWrapper which contains a reference to the underlying MemberInspector which can be used to detect value changes.

## Tags
The inspector can be customized by adding c# tags to members/classes. You can find available tags in the following list:

### General
| Name                | Parameters                    | Targets        | Effect                                                                                     |
|---------------------|-------------------------------|----------------|--------------------------------------------------------------------------------------------|
| **ShowInInspector** | -                             | Field/Property | Shows the member in the inspector                                                          |
| **HideInInspector** | -                             | Field/Property | Hides the member from the inspector                                                        |
| **DisplayName**     | (string) name                 | Field/Property | Alters the displayed name of the member                                                    |
| **ReadOnly**        | -                             | Field/Property | Sets the member to be read only (if not already)                                           |
| **Serializable**    | -                             | Class          | Sets the class to be serializable, enabling save/load buttons being shown in the inspector |
| **Description**     | (string) description          | Field/Property | Adds a description to the member, being displayed when hovered                             |
| **LabelSize**       | (float) sizeMultiplier        | Field/Property | Multiplies the labels width by the given Multiplier                                        |
| **Label**           | (string) text, (int) fontSize | Field/Property | Adds a label above the target field with the given text                                    |
| **Line**            | -                             | Field/Property | Adds a horizontal line above the target field                                              |

### Layout
| Name              | Parameters                                                               | Targets        | Effect                                                                                                                          |
|-------------------|--------------------------------------------------------------------------|----------------|---------------------------------------------------------------------------------------------------------------------------------|
| **PropertyOrder** | (int) order                                                              | Field/Property | Sets the given order (higher means further down)                                                                                |
| **Tab**           | (string) tab                                                             | Field/Property | Moves the property into the given tab                                                                                           |
| **BoxGroup**      | (string) groupName, (LayoutFlags) layoutFlags, (Orientation) orientation | Field/Property | Moves the property into the given group. Orientation specifies horizontal/vertical, layout flags allow for additional layouting |
| **Space**         | (float) topMargin                                                        | Field/Property | Adds additional space above target property                                                                                     |
| **Spacing**       | (int) top, (int) bottom, (int) left, (int) right                         | Field/Property | Adds additional margin around the target field                                                                                  |
| **LayoutFlags**   | (LayoutFlags) flags                                                      | Field/Property | Specifies additional layout flags for the target field                                                                          |


### Specific
| Name           | Parameters                                  | Targets                           | Effect                                                       |
|----------------|---------------------------------------------|-----------------------------------|--------------------------------------------------------------|
| **Checkbox**   | -                                           | Field/Property (bool)             | Switches the default toggle to a checkbox left               |
| **StepSize**   | (double) stepSize                           | Field/Property (int/float/double) | Defines a custom step size for the target range              |
| **Range**      | (double) minIncl, (double) maxIncl          | Field/Property (int/float/double) | Specifies a custom range for the field                       |
| **Slider**     | -                                           | Field/Property (int/float/double) | Switches Spinbox to a slider for target field                |
| **Suffix**     | (string) suffix                             | Field/Property (int/float/double) | Defines a suffix for the target spinbox                      |
| **PathPicker** | (FileModeEnum) fileMode, (string[]) filters | Field/Property (string)           | Adds a button for picking a path with the given mode/filters |

## Supported Types
Aside from all primitive types, the runtime inspector can display:
- Enums
- (Godot-)Colors
- Arrays
- Lists

All classes are displayed as composites of their members.
There is no support for dictionaries at the moment

## Layout Flags
You can specify some additional layout options via the layout flags, which can take effect depending on the element they are used for. The following flags are available:
- **ExpandedInitially:** Expands the group initially
- **NotFoldable:** Prevents the group from being foldable by the user (This overrides ExpandedInitially to be true)
- **NoLabel:** Hides the label of this member
- **NoElements:** Hides some elements, such as save/load buttons for serializable classes
- **NoBackground:** Hides the background for the group

## Value Changes
When a value in the inspector is changed by the user, the value is **not** updated in the underlying instance. Instead a ValueChanged event is emmitted containing a path to the changed member. Upon calling ```TryRetrieveMember()``` values are written back into the instance, as otherwise a deep copy would be required.

Values in the Inspector are polled every second from the displayed instance by default.
Alternatively, a class can implement the ```ITickProvider``` interface to determine when the ui should be updated.
