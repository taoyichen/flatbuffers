Tutorial   {#flatbuffers_guide_tutorial}
========

## Overview

This tutorial provides a basic example of how to work with
[FlatBuffers](@ref flatbuffers_overview). We will step through a simple example
application, which shows you how to:

   - Write a FlatBuffer `schema` file.
   - Use the `flatc` FlatBuffer compiler.
   - Parse [JSON](http://json.org) files that conform to a schema into
     FlatBuffer binary files.
   - Use the generated files in many of the supported languages (such as C++,
     Java, and more.)

During this example, imagine that you are creating a game where the main
character, the hero of the story, needs to slay some `orc`s. We will walk
through each step necessary to create this monster type using FlatBuffers.

Please select your desired language for our quest:

\htmlonly
<form>
  <input type="radio" name="language" value="cpp" checked="checked">C++</input>
  <input type="radio" name="language" value="java">Java</input>
  <input type="radio" name="language" value="csharp">C#</input>
  <input type="radio" name="language" value="go">Go</input>
  <input type="radio" name="language" value="python">Python</input>
  <input type="radio" name="language" value="javascript">JavaScript</input>
  <input type="radio" name="language" value="php">PHP</input>
</form>
\endhtmlonly

\htmlonly
<script>
  /**
   * Check if an HTML `class` attribute is in the language-specific format.
   * @param {string} languageClass An HTML `class` attribute in the format
   * 'language-{lang}', where {lang} is a programming language (e.g. 'cpp',
   * 'java', 'go', etc.).
   * @return {boolean} Returns `true` if `languageClass` was in the valid
   * format, prefixed with 'language-'. Otherwise, it returns false.
   */
  function isProgrammingLanguageClassName(languageClass) {
    if (languageClass && languageClass.substring(0, 9) == 'language-' &&
        languageClass.length > 8) {
      return true;
    } else {
      return false;
    }
  }

  /**
   * Given a language-specific HTML `class` attribute, extract the language.
   * @param {string} languageClass The string name of an HTML `class` attribute,
   * in the format `language-{lang}`, where {lang} is a programming language
   * (e.g. 'cpp', 'java', 'go', etc.).
   * @return {string} Returns a string containing only the {lang} portion of
   * the class name. If the input was invalid, then it returns `null`.
   */
  function extractProgrammingLanguageFromLanguageClass(languageClass) {
    if (isProgrammingLanguageClassName(languageClass)) {
      return languageClass.substring(9);
    } else {
      return null;
    }
  }

  /**
   * Hide every code snippet, except for the language that is selected.
   */
  function displayChosenLanguage() {
    var selection = $('input:checked').val();

    var htmlElements = document.getElementsByTagName('*');
    for (var i = 0; i < htmlElements.length; i++) {
      if (isProgrammingLanguageClassName(htmlElements[i].className)) {
        if (extractProgrammingLanguageFromLanguageClass(
              htmlElements[i].className).toLowerCase() != selection) {
          htmlElements[i].style.display = 'none';
        } else {
          htmlElements[i].style.display = 'initial';
        }
      }
    }
  }

  $( document ).ready(displayChosenLanguage);

  $('input[type=radio]').on("click", displayChosenLanguage);
</script>
\endhtmlonly

## Where to Find the Example Code

Samples demonstating the concepts in this example are located in the source code
package, under the `samples` directory. You can browse the samples on GitHub
[here](https://github.com/google/flatbuffers/tree/master/samples).

For your chosen language, please cross-reference with:

<div class="language-cpp">
[sample_binary.cpp](https://github.com/google/flatbuffers/blob/master/samples/sample_binary.cpp)
</div>
<div class="language-java">
[SampleBinary.java](https://github.com/google/flatbuffers/blob/master/samples/SampleBinary.java)
</div>
<div class="language-csharp">
[SampleBinary.cs](https://github.com/google/flatbuffers/blob/master/samples/SampleBinary.cs)
</div>
<div class="language-go">
[sample_binary.go](https://github.com/google/flatbuffers/blob/master/samples/sample_binary.go)
</div>
<div class="language-python">
[sample_binary.py](https://github.com/google/flatbuffers/blob/master/samples/sample_binary.py)
</div>
<div class="language-javascript">
[samplebinary.js](https://github.com/google/flatbuffers/blob/master/samples/samplebinary.js)
</div>
<div class="language-php">
[SampleBinary.php](https://github.com/google/flatbuffers/blob/master/samples/SampleBinary.php)
</div>

## Writing the Monsters' FlatBuffer Schema

To start working with FlatBuffers, you first need to create a `schema` file,
which defines the format for each data structure you wish to serialize. Here is
the `schema` that defines the template for our monsters:

~~~
  // Example IDL file for our monster's schema.

  namespace MyGame.Sample;

  enum Color:byte { Red = 0, Green, Blue = 2 }

  union Equipment { Weapon } // Optionally add more tables.

  struct Vec3 {
    x:float;
    y:float;
    z:float;
  }

  table Monster {
    pos:Vec3; // Struct.
    mana:short = 150;
    hp:short = 100;
    name:string;
    friendly:bool = false (deprecated);
    inventory:[ubyte];  // Vector of scalars.
    color:Color = Blue; // Enum.
    weapons:[Weapon];   // Vector of tables.
    equipped:Equipment; // Union.
  }

  table Weapon {
    name:string;
    damage:short;
  }

  root_type Monster;
~~~

As you can see, the syntax for the `schema`
[Interface Definition Language (IDL)](https://en.wikipedia.org/wiki/Interface_description_language)
is similar to those of the C family of languages, and other IDL languages. Let's
examine each part of this `schema` to determine what it does.

The `schema` starts with a `namespace` declaration. This determines the
corresponding package/namespace for the generated code. In our example, we have
the `Sample` namespace inside of the `MyGame` namespace.

Next, we have an `enum` definition. In this example, we have an `enum` of type
`byte`, named `Color`. We have three values in this `enum`: `Red`, `Green`, and
`Blue`. We specify `Red = 0` and `Blue = 2`, but we do not specify an explicit
value for `Green`. Since the behavior of an `enum` is to increment if
unspecified, `Green` will receive the implicit value of `1`.

Following the `enum` is a `union`. The `union` in this example is not very
useful, as it only contains the one `table` (named `Weapon`). If we had created
multiple tables that we would want the `union` to be able to reference, we
could add more elements to the `union Equipment`.

After the `union` comes a `struct Vec3`, which represents a floating point
vector with `3` dimensions. We use a `struct` here, over a `table`, because
`struct`s are ideal for data structures that will not change, since they use
less memory and have faster lookup.

The `Monster` table is the main object in our FlatBuffer. This will be used as
the template to store our `orc` monster. We specify some default values for
fields, such as `mana:short = 150`. All unspecified fields will default to `0`
or `NULL`. Another thing to note is the line
`friendly:bool = false (deprecated);`. Since you cannot delete fields from a
`table` (to support backwards compatability), you can set fields as
`deprecated`, which will prevent the generation of accessors for this field in
the generated code. Be careful when using `deprecated`, however, as it may break
legacy code that used this accessor.

The `Weapon` table is a sub-table used within our FlatBuffer. It is
used twice: once within the `Monster` table and once within the `Equipment`
enum. For our `Monster`, it is used to populate a `vector of tables` via the
`weapons` field within our `Monster`. It is also the only table referenced by
the `Equipment` enum.

The last part of the `schema` is the `root_type`. The root type declares what
will be the root table for the serialized data. In our case, the root type is
our `Monster` table.

#### More Information About Schemas

You can find a complete guide to writing `schema` files in the
[Writing a schema](@ref flatbuffers_guide_writing_schema) section of the
Programmer's Guide. You can also view the formal
[Grammar of the schema language](@ref flatbuffers_grammar).

## Compiling the Monsters' Schema

After you have written the FlatBuffers schema, the next step is to compile it.

If you have not already done so, please follow
[these instructions](@ref flatbuffers_guide_building) to build `flatc`, the
FlatBuffer compiler.

Once `flatc` is built successfully, compile the schema for your language:

<div class="language-cpp">
~~~{.sh}
  cd flatbuffers/sample
  ./../flatc --cpp samples/monster.fbs
~~~
</div>
<div class="language-java">
~~~{.sh}
  cd flatbuffers/sample
  ./../flatc --java samples/monster.fbs
~~~
</div>
<div class="language-csharp">
~~~{.sh}
  cd flatbuffers/sample
  ./../flatc --csharp samples/monster.fbs
~~~
</div>
<div class="language-go">
~~~{.sh}
  cd flatbuffers/sample
  ./../flatc --go samples/monster.fbs
~~~
</div>
<div class="language-python">
~~~{.sh}
  cd flatbuffers/sample
  ./../flatc --python samples/monster.fbs
~~~
</div>
<div class="language-javascript">
~~~{.sh}
  cd flatbuffers/sample
  ./../flatc --javascript samples/monster.fbs
~~~
</div>
<div class="language-php">
~~~{.sh}
  cd flatbuffers/sample
  ./../flatc --php samples/monster.fbs
~~~
</div>

For a more complete guide to using the `flatc` compiler, pleaes read the
[Using the schema compiler](@ref flatbuffers_guide_using_schema_compiler)
section of the Programmer's Guide.

## Reading and Writing Monster FlatBuffers

Now that we have compiled the schema for our programming language, we can
start creating some monsters and serializing/deserializing them from
FlatBuffers.

#### Creating and Writing Orc FlatBuffers

The first step is to import/include the library, generated files, etc.

<div class="language-cpp">
~~~{.cpp}
  #include "monster_generate.h" // This was generated by `flatc`.

  using namespace MyGame::Sample; // Specified in the schema.
~~~
</div>
<div class="language-java">
~~~{.java}
  import MyGame.Sample.*; //The `flatc` generated files. (Monster, Vec3, etc.)

  import com.google.flatbuffers.FlatBufferBuilder;
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  using FlatBuffers;
  using MyGame.Sample; // The `flatc` generated files. (Monster, Vec3, etc.)
~~~
</div>
<div class="language-go">
~~~{.go}
  import (
          flatbuffers "github.com/google/flatbuffers/go"
          sample "MyGame/Sample"
  )
~~~
</div>
<div class="language-python">
~~~{.py}
  import flatbuffers

  # Generated by `flatc`.
  import MyGame.Sample.Color
  import MyGame.Sample.Equipment
  import MyGame.Sample.Monster
  import MyGame.Sample.Vec3
  import MyGame.Sample.Weapon
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // The following code is for JavaScript module loaders (e.g. Node.js). See
  // below for a browser-based HTML/JavaScript example of including the library.
  var flatbuffers = require('/js/flatbuffers').flatbuffers;
  var MyGame = require('./monster_generated').MyGame; // Generated by `flatc`.

  //--------------------------------------------------------------------------//

  // The following code is for browser-based HTML/JavaScript. Use the above code
  // for JavaScript module loaders (e.g. Node.js).
  <script src="../js/flatbuffers.js"></script>
  <script src="monster_generated.js"></script> // Generated by `flatc`.
~~~
</div>
<div class="language-php">
~~~{.php}
  // It is recommended that your use PSR autoload when using FlatBuffers in PHP.
  // Here is an example from `SampleBinary.php`:
  function __autoload($class_name) {
    // The last segment of the class name matches the file name.
    $class = substr($class_name, strrpos($class_name, "\\") + 1);
    $root_dir = join(DIRECTORY_SEPARATOR, array(dirname(dirname(__FILE__)))); // `flatbuffers` root.

    // Contains the `*.php` files for the FlatBuffers library and the `flatc` generated files.
    $paths = array(join(DIRECTORY_SEPARATOR, array($root_dir, "php")),
                   join(DIRECTORY_SEPARATOR, array($root_dir, "samples", "MyGame", "Sample")));
    foreach ($paths as $path) {
      $file = join(DIRECTORY_SEPARATOR, array($path, $class . ".php"));
      if (file_exists($file)) {
        require($file);
        break;
      }
    }
  }
~~~
</div>

Now we are ready to start building some buffers. In order to start, we need
to create an instance of the `FlatBufferBuilder`, which will contain the buffer
as it grows:

<div class="language-cpp">
~~~{.cpp}
  // Create a `FlatBufferBuilder`, which will be used to create our
  // monsters' FlatBuffers.
  flatbuffers::FlatBufferBuilder builder;
~~~
</div>
<div class="language-java">
~~~{.java}
  // Create a `FlatBufferBuilder`, which will be used to create our
  // monsters' FlatBuffers.
  FlatBufferBuilder builder = new FlatBufferBuilder(0);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // Create a `FlatBufferBuilder`, which will be used to create our
  // monsters' FlatBuffers.
  var builder = new FlatBufferBuilder(1);
~~~
</div>
<div class="language-go">
~~~{.go}
  // Create a `FlatBufferBuilder`, which will be used to create our
  // monsters' FlatBuffers.
  builder := flatbuffers.NewBuilder(0)
~~~
</div>
<div class="language-python">
~~~{.py}
  # Create a `FlatBufferBuilder`, which will be used to create our
  # monsters' FlatBuffers.
  builder = flatbuffers.Builder(0)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // Create a `flatbuffer.Builder`, which will be used to create our
  // monsters' FlatBuffers.
  var builder = new flatbuffers.Builder(1);
~~~
</div>
<div class="language-php">
~~~{.php}
  // Create a `FlatBufferBuilder`, which will be used to create our
  // monsters' FlatBuffers.
  $builder = new Google\FlatBuffers\FlatbufferBuilder(0);
~~~
</div>

After creating the `builder`, we can start serializing our data. Before we make
our `orc` Monster, lets create some `Weapon`s: a `Sword` and an `Axe`.

<div class="language-cpp">
~~~{.cpp}
  auto weapon_one_name = builder.CreateString("Sword");
  short weapon_one_damage = 3;

  auto weapon_two_name = builder.CreateString("Axe");
  short weapon_two_damage = 5;

  // Use the `CreateWeapon` shortcut to create Weapons with all the fields set.
  auto sword = CreateWeapon(builder, weapon_one_name, weapon_one_damage);
  auto axe = CreateWeapon(builder, weapon_two_name, weapon_two_damage);
~~~
</div>
<div class="language-java">
~~~{.java}
  int weaponOneName = builder.createString("Sword")
  short weaponOneDamage = 3;

  int weaponTwoName = builder.createString("Axe");
  short weaponTwoDamage = 5;

  // Use the `createWeapon()` helper function to create the weapons, since we set every field.
  int sword = Weapon.createWeapon(builder, weaponOneName, weaponOneDamage);
  int axe = Weapon.createWeapon(builder, weaponTwoName, weaponTwoDamage);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  var weaponOneName = builder.CreateString("Sword");
  var weaponOneDamage = 3;

  var weaponTwoName = builder.CreateString("Axe");
  var weaponTwoDamage = 5;

  // Use the `CreateWeapon()` helper function to create the weapons, since we set every field.
  var sword = Weapon.CreateWeapon(builder, weaponOneName, (short)weaponOneDamage);
  var axe = Weapon.CreateWeapon(builder, weaponTwoName, (short)weaponTwoDamage);
~~~
</div>
<div class="language-go">
~~~{.go}
  weaponOne := builder.CreateString("Sword")
  weaponTwo := builder.CreateString("Axe")

  // Create the first `Weapon` ("Sword").
  sample.WeaponStart(builder)
  sample.Weapon.AddName(builder, weaponOne)
  sample.Weapon.AddDamage(builder, 3)
  sword := sample.WeaponEnd(builder)

  // Create the second `Weapon` ("Axe").
  sample.WeaponStart(builder)
  sample.Weapon.AddName(builder, weaponTwo)
  sample.Weapon.AddDamage(builder, 5)
  axe := sample.WeaponEnd(builder)
~~~
</div>
<div class="language-python">
~~~{.py}
  weapon_one = builder.CreateString('Sword')
  weapon_two = builder.CreateString('Axe')

  # Create the first `Weapon` ('Sword').
  MyGame.Sample.Weapon.WeaponStart(builder)
  MyGame.Sample.Weapon.WeaponAddName(builder, weapon_one)
  MyGame.Sample.Weapon.WeaponAddDamage(builder, 3)
  sword = MyGame.Sample.Weapon.WeaponEnd(builder)

  # Create the second `Weapon` ('Axe').
  MyGame.Sample.Weapon.WeaponStart(builder)
  MyGame.Sample.Weapon.WeaponAddName(builder, weapon_two)
  MyGame.Sample.Weapon.WeaponAddDamage(builder, 5)
  axe = MyGame.Sample.Weapon.WeaponEnd(builder)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  var weaponOne = builder.createString('Sword');
  var weaponTwo = builder.createString('Axe');

  // Create the first `Weapon` ('Sword').
  MyGame.Sample.Weapon.startWeapon(builder);
  MyGame.Sample.Weapon.addName(builder, weaponOne);
  MyGame.Sample.Weapon.addDamage(builder, 3);
  var sword = MyGame.Sample.Weapon.endWeapon(builder);

  // Create the second `Weapon` ('Axe').
  MyGame.Sample.Weapon.startWeapon(builder);
  MyGame.Sample.Weapon.addName(builder, weaponTwo);
  MyGame.Sample.Weapon.addDamage(builder, 5);
  var axe = MyGame.Sample.Weapon.endWeapon(builder);
~~~
</div>
<div class="language-php">
~~~{.php}
  // Create the `Weapon`s using the `createWeapon()` helper function.
  $weapon_one_name = $builder->createString("Sword");
  $sword = \MyGame\Sample\Weapon::CreateWeapon($builder, $weapon_one_name, 3);

  $weapon_two_name = $builder->createString("Axe");
  $axe = \MyGame\Sample\Weapon::CreateWeapon($builder, $weapon_two_name, 5);

  // Create an array from the two `Weapon`s and pass it to the
  // `CreateWeaponsVector()` method to create a FlatBuffer vector.
  $weaps = array($sword, $axe);
  $weapons = \MyGame\Sample\Monster::CreateWeaponsVector($builder, $weaps);
~~~
</div>

Now let's create our monster, the `orc`. For this `orc`, lets make him
`red` with rage, positioned at `(1.0, 2.0, 3.0)`, and give him
a large pool of hit points with `300`. We can give him a vector of weapons
to choose from (our `Sword` and `Axe` from earlier). In this case, we will
equip him with the `Axe`, since it is the most powerful of the two. Lastly,
let's fill his inventory with some potential treasures that can be taken once he
is defeated.

Before we serialize a monster, we need to first serialize any objects that are
contained there-in, i.e. we serialize the data tree using depth-first, pre-order
traversal. This is generally easy to do on any tree structures.

<div class="language-cpp">
~~~{.cpp}
  // Serialize a name for our monster, called "Orc".
  auto name = builder.CreateString("Orc");

  // Create a `vector` representing the inventory of the Orc. Each number
  // could correspond to an item that can be claimed after he is slain.
  unsigned char treasure = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
  auto inventory = builder.CreateVector(treasure, 10);
~~~
</div>
<div class="language-java">
~~~{.java}
  // Serialize a name for our monster, called "Orc".
  int name = builder.createString("Orc");

  // Create a `vector` representing the inventory of the Orc. Each number
  // could correspond to an item that can be claimed after he is slain.
  byte[] treasure = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
  int inv = Monster.createInventoryVector(builder, treasure);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // Serialize a name for our monster, called "Orc".
  var name = builder.CreateString("Orc");

  // Create a `vector` representing the inventory of the Orc. Each number
  // could correspond to an item that can be claimed after he is slain.
  // Note: Since we prepend the bytes, this loop iterates in reverse order.
  Monster.StartInventoryVector(builder, 10);
  for (int i = 9; i >= 0; i--)
  {
    builder.AddByte((byte)i);
  }
  var inv = builder.EndVector();
~~~
</div>
<div class="language-go">
~~~{.go}
  // Serialize a name for our monster, called "Orc".
  name := builder.CreateString("Orc")

  // Create a `vector` representing the inventory of the Orc. Each number
  // could correspond to an item that can be claimed after he is slain.
  // Note: Since we prepend the bytes, this loop iterates in reverse.
  sample.MonsterStartInventoryVector(builder, 10)
  for i := 9; i >= 0; i-- {
          builder.PrependByte(byte(i))
  }
  int := builder.EndVector(10)
~~~
</div>
<div class="language-python">
~~~{.py}
  # Serialize a name for our monster, called "Orc".
  name = builder.CreateString("Orc")

  # Create a `vector` representing the inventory of the Orc. Each number
  # could correspond to an item that can be claimed after he is slain.
  # Note: Since we prepend the bytes, this loop iterates in reverse.
  MyGame.Sample.Monster.MonsterStartInventoryVector(builder, 10)
  for i in reversed(range(0, 10)):
    builder.PrependByte(i)
  inv = builder.EndVector(10)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // Serialize a name for our monster, called 'Orc'.
  var name = builder.createString('Orc');

  // Create a `vector` representing the inventory of the Orc. Each number
  // could correspond to an item that can be claimed after he is slain.
  var treasure = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
  var inv = MyGame.Sample.Monster.createInventoryVector(builder, treasure);
~~~
</div>
<div class="language-php">
~~~{.php}
  // Serialize a name for our monster, called "Orc".
  $name = $builder->createString("Orc");

  // Create a `vector` representing the inventory of the Orc. Each number
  // could correspond to an item that can be claimed after he is slain.
  $treasure = array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
  $inv = \MyGame\Sample\Monster::CreateInventoryVector($builder, $treasure);
~~~
</div>

We serialized two built-in data types (`string` and `vector`) and captured
their return values. These values are offsets into the serialized data,
indicating where they are stored, such that we can refer to them below when
adding fields to our monster.

*Note: To create a `vector` of nested objects (e.g. `table`s, `string`s, or
other `vector`s), collect their offsets into a temporary data structure, and
then create an additional `vector` containing their offsets.*

For example, take a look at the two `Weapon`s that we created earlier (`Sword`
and `Axe`). These are both FlatBuffer `table`s, whose offsets we now store in
memory. Therefore we can create a FlatBuffer `vector` to contain these
offsets.

<div class="language-cpp">
~~~{.cpp}
  // Place the weapons into a `std::vector`, then convert that into a FlatBuffer `vector`.
  std::vector<flatbuffers::Offset<Weapon>> weapons_vector;
  weapons_vector.push_back(sword);
  weapons_vector.push_back(axe);
  auto weapons = builder.CreateVector(weapons_vector);
~~~
</div>
<div class="language-java">
~~~{.java}
  // Place the two weapons into an array, and pass it to the `createWeaponsVector()` method to
  // create a FlatBuffer vector.
  int[] weaps = new int[2];
  weaps[1] = sword;
  weaps[2] = axe;

  // Pass the `weaps` array into the `createWeaponsVector()` method to create a FlatBuffer vector.
  int weapons = Monster.createWeaponsVector(builder, weaps);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  var weaps = new Offset<Weapon>[2];
  weaps[0] = sword;
  weaps[1] = axe;

  // Pass the `weaps` array into the `CreateWeaponsVector()` method to create a FlatBuffer vector.
  var weapons = Monster.CreateWeaponsVector(builder, weaps);
~~~
</div>
<div class="language-go">
~~~{.go}
  // Create a FlatBuffer vector and prepend the weapons.
  // Note: Since we prepend the data, prepend them in reverse order.
  sample.MonsterStartWeaponsVector(builder, 2)
  builder.PrependUOffsetT(axe)
  builder.PrependUOffsetT(sword)
  weapons := builder.EndVector(2)
~~~
</div>
<div class="language-python">
~~~{.py}
  # Create a FlatBuffer vector and prepend the weapons.
  # Note: Since we prepend the data, prepend them in reverse order.
  MyGame.Sample.Monster.MonsterStartWeaponsVector(builder, 2)
  builder.PrependUOffsetTRelative(axe)
  builder.PrependUOffsetTRelative(sword)
  weapons = builder.EndVector(2)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // Create an array from the two `Weapon`s and pass it to the
  // `createWeaponsVector()` method to create a FlatBuffer vector.
  var weaps = [sword, axe];
  var weapons = MyGame.Sample.Monster.createWeaponsVector(builder, weaps);
~~~
</div>
<div class="language-php">
~~~{.php}
  // Create an array from the two `Weapon`s and pass it to the
  // `CreateWeaponsVector()` method to create a FlatBuffer vector.
  $weaps = array($sword, $axe);
  $weapons = \MyGame\Sample\Monster::CreateWeaponsVector($builder, $weaps);
~~~
</div>

To create a `struct`, use the `Vec3` class/struct that was generated by `flatc`:

<div class="language-cpp">
~~~{.cpp}
  // Create a `Vec3`, representing the Orc's position in 3-D space.
  auto pos = Vec3(1.0f, 2.0f, 3.0f);
~~~
</div>
<div class="language-java">
~~~{.java}
  // Create a `Vec3`, representing the Orc's position in 3-D space.
  int pos = Vec3.createVec3(builder, 1.0f, 2.0f, 3.0f);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // Create a `Vec3`, representing the Orc's position in 3-D space.
  var pos = Vec3.CreateVec3(builder, 1.0f, 2.0f, 3.0f);
~~~
</div>
<div class="language-go">
~~~{.go}
  // Create a `Vec3`, representing the Orc's position in 3-D space.
  pos := sample.CreateVec3(builder, 1.0, 2.0, 3.0)
~~~
</div>
<div class="language-python">
~~~{.py}
  # Create a `Vec3`, representing the Orc's position in 3-D space.
  pos = MyGame.Sample.Vec3.CreateVec3(builder, 1.0, 2.0, 3.0)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // Create a `Vec3`, representing the Orc's position in 3-D space.
  var pos = MyGame.Sample.Vec3.createVec3(builder, 1.0, 2.0, 3.0);
~~~
</div>
<div class="language-php">
~~~{.js}
  // Create a `Vec3`, representing the Orc's position in 3-D space.
  $pos = \MyGame\Sample\Vec3::CreateVec3($builder, 1.0, 2.0, 3.0);
~~~
</div>

We have now serialized the non-scalar components of the orc, so we
can serialize the monster itself:

<div class="language-cpp">
~~~{.cpp}
  // Set his hit points to 300 and his mana to 150.
  int hp = 300;
  int mana = 150;

  // Finally, create the monster using the `CreateMonster` helper function
  // to set all fields.
  auto orc = CreateMonster(builder, &pos, mana, hp, name, inventory, Color_Red,
                           weapons, Equipment_Weapon, axe.Union());
~~~
</div>
<div class="language-java">
~~~{.java}
  // Create our monster using `startMonster()` and `endMonster()`.
  Monster.startMonster(builder);
  Monster.addPos(builder, pos);
  Monster.addName(builder, name);
  Monster.addColor(builder, Color.Red);
  Monster.addHp(builder, (short)300);
  Monster.addInventory(builder, inv);
  Monster.addWeapons(builder, weapons);
  Monster.addEquippedType(builder, Equipment.Weapon);
  Monster.addEquipped(builder, axe);
  int orc = Monster.endMonster(builder);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // Create our monster using `StartMonster()` and `EndMonster()`.
  Monster.StartMonster(builder);
  Monster.AddPos(builder, pos);
  Monster.AddHp(builder, (short)300);
  Monster.AddName(builder, name);
  Monster.AddInventory(builder, inv);
  Monster.AddColor(builder, Color.Red);
  Monster.AddWeapons(builder, weapons);
  Monster.AddEquippedType(builder, Equipment.Weapon);
  Monster.AddEquipped(builder, axe.Value); // Axe
  var orc = Monster.EndMonster(builder);
~~~
</div>
<div class="language-go">
~~~{.go}
  // Create our monster using `MonsterStart()` and `MonsterEnd()`.
  sample.MonsterStart(builder)
  sample.MonsterAddPos(builder, pos)
  sample.MonsterAddHp(builder, 300)
  sample.MonsterAddName(builder, name)
  sample.MonsterAddInventory(builder, inv)
  sample.MonsterAddColor(builder, sample.ColorRed)
  sample.MonsterAddWeapons(builder, weapons)
  sample.MonsterAddEquippedType(builder, sample.EquipmentWeapon)
  sample.MonsterAddEquipped(builder, axe)
  orc := sample.MonsterEnd(builder)
~~~
</div>
<div class="language-python">
~~~{.py}
  # Create our monster by using `MonsterStart()` and `MonsterEnd()`.
  MyGame.Sample.Monster.MonsterStart(builder)
  MyGame.Sample.Monster.MonsterAddPos(builder, pos)
  MyGame.Sample.Monster.MonsterAddHp(builder, 300)
  MyGame.Sample.Monster.MonsterAddName(builder, name)
  MyGame.Sample.Monster.MonsterAddInventory(builder, inv)
  MyGame.Sample.Monster.MonsterAddColor(builder,
                                        MyGame.Sample.Color.Color().Red)
  MyGame.Sample.Monster.MonsterAddWeapons(builder, weapons)
  MyGame.Sample.Monster.MonsterAddEquippedType(
      builder, MyGame.Sample.Equipment.Equipment().Weapon)
  MyGame.Sample.Monster.MonsterAddEquipped(builder, axe)
  orc = MyGame.Sample.Monster.MonsterEnd(builder)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // Create our monster by using `startMonster()` and `endMonster()`.
  MyGame.Sample.Monster.startMonster(builder);
  MyGame.Sample.Monster.addPos(builder, pos);
  MyGame.Sample.Monster.addHp(builder, 300);
  MyGame.Sample.Monster.addColor(builder, MyGame.Sample.Color.Red)
  MyGame.Sample.Monster.addName(builder, name);
  MyGame.Sample.Monster.addInventory(builder, inv);
  MyGame.Sample.Monster.addWeapons(builder, weapons);
  MyGame.Sample.Monster.addEquippedType(builder, MyGame.Sample.Equipment.Weapon);
  MyGame.Sample.Monster.addEquipped(builder, axe);
  var orc = MyGame.Sample.Monster.endMonster(builder);
~~~
</div>
<div class="language-php">
~~~{.php}
  // Create our monster by using `StartMonster()` and `EndMonster()`.
  \MyGame\Sample\Monster::StartMonster($builder);
  \MyGame\Sample\Monster::AddPos($builder, $pos);
  \MyGame\Sample\Monster::AddHp($builder, 300);
  \MyGame\Sample\Monster::AddName($builder, $name);
  \MyGame\Sample\Monster::AddInventory($builder, $inv);
  \MyGame\Sample\Monster::AddColor($builder, \MyGame\Sample\Color::Red);
  \MyGame\Sample\Monster::AddWeapons($builder, $weapons);
  \MyGame\Sample\Monster::AddEquippedType($builder, \MyGame\Sample\Equipment::Weapon);
  \MyGame\Sample\Monster::AddEquipped($builder, $axe);
  $orc = \MyGame\Sample\Monster::EndMonster($builder);
~~~
</div>

<div class="language-cpp">
<br>
*Note: Since we passing `150` as the `mana` field, which happens to be the
default value, the field will not actually be written to the buffer, since the
default value will be returned on query anyway. This is a nice space savings,
especially if default values are common in your data. It also means that you do
not need to be worried of adding a lot of fields that are only used in a small
number of instances, as it will not bloat the buffer if unused.*
<br><br>
If you do not wish to set every field in a `table`, it may be more convenient to
manually set each field of your monster, instead of calling `CreateMonster()`.
The following snippet is functionally equivalent to the above code, but provides
a bit more flexibility.
<br>
~~~{.cpp}
  // You can use this code instead of `CreateMonster()`, to create our orc
  // manually.
  MonsterBuilder monster_builder(builder);
  monster_builder.add_pos(&pos);
  monster_builder.add_hp(hp);
  monster_builder.add_name(name);
  monster_builder.add_inventory(inventory);
  monster_builder.add_color(Color_Red);
  monster_builder.add_weapons(weapons);
  monster_builder.add_equipped_type(Equipment_Weapon);
  monster_builder.add_equpped(axe);
  auto orc = monster_builder.Finish();
~~~
</div>

Before finishing the serialization, let's take a quick look at FlatBuffer
`union Equipped`. There are two parts to each FlatBuffer `union`. The first, is
a hidden field `_type`, that is generated to hold the type of `table` referred
to by the `union`. This allows you to know which type to cast to at runtime.
Second, is the `union`'s data.

In our example, the last two things we added to our `Monster` were the
`Equipped Type` and the `Equipped` union itself.

Here is a repetition these lines, to help highlight them more clearly:

<div class="language-cpp">
  ~~~{.cpp}
    monster_builder.add_equipped_type(Equipment_Weapon); // Union type
    monster_builder.add_equipped(axe); // Union data
  ~~~
</div>
<div class="language-java">
  ~~~{.java}
    Monster.addEquippedType(builder, Equipment.Weapon); // Union type
    Monster.addEquipped(axe); // Union data
  ~~~
</div>
<div class="language-csharp">
  ~~~{.cs}
    Monster.AddEquippedType(builder, Equipment.Weapon); // Union type
    Monster.AddEquipped(builder, axe.Value); // Union data
  ~~~
</div>
<div class="language-go">
  ~~~{.go}
    sample.MonsterAddEquippedType(builder, sample.EquipmentWeapon) // Union type
    sample.MonsterAddEquipped(builder, axe) // Union data
  ~~~
</div>
<div class="language-python">
  ~~~{.py}
    MyGame.Sample.Monster.MonsterAddEquippedType(            # Union type
        builder, MyGame.Sample.Equipment.Equipment().Weapon)
    MyGame.Sample.Monster.MonsterAddEquipped(builder, axe)   # Union data
  ~~~
</div>
<div class="language-javascript">
  ~~~{.js}
    MyGame.Sample.Monster.addEquippedType(builder, MyGame.Sample.Equipment.Weapon); // Union type
    MyGame.Sample.Monster.addEquipped(builder, axe); // Union data
  ~~~
</div>
<div class="language-php">
  ~~~{.php}
    \MyGame\Sample\Monster::AddEquippedType($builder, \MyGame\Sample\Equipment::Weapon); // Union type
    \MyGame\Sample\Monster::AddEquipped($builder, $axe); // Union data
  ~~~
</div>

After you have created your buffer, you will have the offset to the root of the
data in the `orc` variable, so you can finish the buffer by calling the
appropriate `finish` method.

<div class="language-cpp">
~~~{.cpp}
  // Call `Finish()` to instruct the builder that this monster is complete.
  // Note: Regardless of how you created the `orc`, you still need to call
  // `Finish()` on the `FlatBufferBuilder`.
  builder.Finish(orc); // You could also call `FinishMonsterBuffer(builder,
                       //                                          orc);`.
~~~
</div>
<div class="language-java">
~~~{.java}
  // Call `finish()` to instruct the builder that this monster is complete.
  builder.finish(orc); // You could also call `Monster.finishMonsterBuffer(builder, orc);`.
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // Call `Finish()` to instruct the builder that this monster is complete.
  builder.Finish(orc.Value); // You could also call `Monster.FinishMonsterBuffer(builder, orc);`.
~~~
</div>
<div class="language-go">
~~~{.go}
  // Call `Finish()` to instruct the builder that this monster is complete.
  builder.Finish(orc)
~~~
</div>
<div class="language-python">
~~~{.py}
  # Call `Finish()` to instruct the builder that this monster is complete.
  builder.Finish(orc)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // Call `finish()` to instruct the builder that this monster is complete.
  builder.finish(orc); // You could also call `MyGame.Example.Monster.finishMonsterBuffer(builder,
                       //                                                                 orc);`.
~~~
</div>
<div class="language-php">
~~~{.php}
  // Call `finish()` to instruct the builder that this monster is complete.
   $builder->finish($orc); // You may also call `\MyGame\Sample\Monster::FinishMonsterBuffer(
                           //                        $builder, $orc);`.
~~~
</div>

The buffer is now ready to be stored somewhere, sent over the network, be
compressed, or whatever you'd like to do with it. You can access the buffer
like so:

<div class="language-cpp">
~~~{.cpp}
  // This must be called after `Finish()`.
  uint8_t *buf = builder.GetBufferPointer();
  int size = builder.GetSize(); // Returns the size of the buffer that
                                // `GetBufferPointer()` points to.
~~~
</div>
<div class="language-java">
~~~{.java}
  // This must be called after `finish()`.
  java.nio.ByteBuffer buf = builder.dataBuffer();
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // This must be called after `Finish()`.
  var buf = builder.DataBuffer; // Of type `FlatBuffers.ByteBuffer`.
~~~
</div>
<div class="language-go">
~~~{.go}
  // This must be called after `Finish()`.
  buf := builder.FinishedBytes() // Of type `byte[]`.
~~~
</div>
<div class="language-python">
~~~{.py}
  # This must be called after `Finish()`.
  buf = builder.Output() // Of type `bytearray`.
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // This must be called after `finish()`.
  var buf = builder.dataBuffer(); // Of type `flatbuffers.ByteBuffer`.
~~~
</div>
<div class="language-php">
~~~{.php}
  // This must be called after `finish()`.
  $buf = $builder->dataBuffer(); // Of type `Google\FlatBuffers\ByteBuffer`
~~~
</div>

#### Reading Orc FlatBuffers

Now that we have successfully created an `Orc` FlatBuffer, the monster data can
be saved, sent over a network, etc. Let's now adventure into the inverse, and
deserialize a FlatBuffer.

This seciton requires the same import/include, namespace, etc. requirements as
before:

<div class="language-cpp">
~~~{.cpp}
  #include "monster_generate.h" // This was generated by `flatc`.

  using namespace MyGame::Sample; // Specified in the schema.
~~~
</div>
<div class="language-java">
~~~{.java}
  import MyGame.Sample.*; //The `flatc` generated files. (Monster, Vec3, etc.)

  import com.google.flatbuffers.FlatBufferBuilder;
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  using FlatBuffers;
  using MyGame.Sample; // The `flatc` generated files. (Monster, Vec3, etc.)
~~~
</div>
<div class="language-go">
~~~{.go}
  import (
          flatbuffers "github.com/google/flatbuffers/go"
          sample "MyGame/Sample"
  )
~~~
</div>
<div class="language-python">
~~~{.py}
  import flatbuffers

  # Generated by `flatc`.
  import MyGame.Sample.Any
  import MyGame.Sample.Color
  import MyGame.Sample.Monster
  import MyGame.Sample.Vec3
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // The following code is for JavaScript module loaders (e.g. Node.js). See
  // below for a browser-based HTML/JavaScript example of including the library.
  var flatbuffers = require('/js/flatbuffers').flatbuffers;
  var MyGame = require('./monster_generated').MyGame; // Generated by `flatc`.

  //--------------------------------------------------------------------------//

  // The following code is for browser-based HTML/JavaScript. Use the above code
  // for JavaScript module loaders (e.g. Node.js).
  <script src="../js/flatbuffers.js"></script>
  <script src="monster_generated.js"></script> // Generated by `flatc`.
~~~
</div>
<div class="language-php">
~~~{.php}
  // It is recommended that your use PSR autoload when using FlatBuffers in PHP.
  // Here is an example from `SampleBinary.php`:
  function __autoload($class_name) {
    // The last segment of the class name matches the file name.
    $class = substr($class_name, strrpos($class_name, "\\") + 1);
    $root_dir = join(DIRECTORY_SEPARATOR, array(dirname(dirname(__FILE__)))); // `flatbuffers` root.

    // Contains the `*.php` files for the FlatBuffers library and the `flatc` generated files.
    $paths = array(join(DIRECTORY_SEPARATOR, array($root_dir, "php")),
                   join(DIRECTORY_SEPARATOR, array($root_dir, "samples", "MyGame", "Sample")));
    foreach ($paths as $path) {
      $file = join(DIRECTORY_SEPARATOR, array($path, $class . ".php"));
      if (file_exists($file)) {
        require($file);
        break;
      }
    }
  }
~~~
</div>

Then, assuming you have a variable containing to the bytes of data from disk,
network, etc., you can create a monster from this data:

<div class="language-cpp">
~~~{.cpp}
  // We can access the buffer we just made directly. Pretend this came over a
  // network, was read off of disk, etc.
  auto buffer_pointer = builder.GetBufferPointer();

  // Deserialize the data from the buffer.
  auto monster = GetMonster(buffer_pointer);

  // `monster` is of type`Monster *`, and points to somewhere inside the buffer.

  // Note: root object pointers are NOT the same as `buffer_pointer`.
~~~
</div>
<div class="language-java">
~~~{.java}
  // We can access the buffer we just made directly. Pretend this came over a
  // network, was read off of disk, etc.
  java.nio.ByteBuffer buf = builder.dataBuffer();

  // Deserialize the data from the buffer.
  Monster monster = Monster.getRootAsMonster(buf);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // We can access the buffer we just made directly. Pretend this came over a
  // network, was read off of disk, etc.
  var buf = builder.DataBuffer;

  // Deserialize the data from the buffer.
  var monster = Monster.GetRootAsMonster(buf);
~~~
</div>
<div class="language-go">
~~~{.go}
  // We can access the buffer we just made directly. Pretend this came over a
  // network, was read off of disk, etc.
  buf := builder.FinishedBytes()

  // Deserialize the data from the buffer.
  monster := sample.GetRootAsMonster(buf, 0)

  // Note: We use `0` for the offset here, since we got the data using the
  // `builder.FinishedBytes()` method. This simulates the data you would
  // store/receive in your FlatBuffer. If you wanted to read from the
  // `builder.Bytes` directly, you would need to pass in the offset of
  // `builder.Head()`, as the builder actually constructs the buffer backwards.
~~~
</div>
<div class="language-python">
~~~{.py}
  # We can access the buffer we just made directly. Pretend this came over a
  # network, was read off of disk, etc.
  buf = builder.Output()

  # Deserialize the data from the buffer.
  monster = MyGame.Sample.Monster.Monster.GetRootAsMonster(buf, 0)

  # Note: We use `0` for the offset here, since we got the data using the
  # `builder.Output()` method. This simulates the data you would store/receive
  # in your FlatBuffer. If you wanted to read from the `builder.Bytes` directly,
  # you would need to pass in the offset of `builder.Head()`, as the builder
  # actually constructs the buffer backwards.
~~~
</div>
<div class="language-javascript">
~~~{.js}
  // We can access the buffer we just made directly. Pretend this came over a
  // network, was read off of disk, etc.
  var buf = builder.dataBuffer();

  // Deserialize the data from the buffer.
  var monster = MyGame.Sample.Monster.getRootAsMonster(buf);
~~~
</div>
<div class="language-php">
~~~{.php}
  // We can access the buffer we just made directly. Pretend this came over a
  // network, was read off of disk, etc.
  $buf = $builder->dataBuffer();

  // Deserialize the data from the buffer.
  $monster = \MyGame\Sample\Monster::GetRootAsMonster($buf);
~~~
</div>

If you look in the generated files from `flatc`, you will see it generated
accessors for all non-`deprecated` fields. For example:

<div class="language-cpp">
~~~{.cpp}
  auto hp = monster->hp();
  auto mana = monster->mana();
  auto name = monster->name()->c_str();
~~~
</div>
<div class="language-java">
~~~{.java}
  short hp = monster.hp();
  short mana = monster.mana();
  String name = monster.name();
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  // For C#, unlike other languages support by FlatBuffers, most values (except for
  // vectors and unions) are available as propreties instead of asccessor methods.
  var hp = monster.Hp
  var mana = monster.Mana
  var name = monster.Name
~~~
</div>
<div class="language-go">
~~~{.go}
  hp := monster.Hp()
  mana := monster.Mana()
  name := string(monster.Name()) // Note: `monster.Name()` returns a byte[].
~~~
</div>
<div class="language-python">
~~~{.py}
  hp = monster.Hp()
  mana = monster.Mana()
  name = monster.Name()
~~~
</div>
<div class="language-javascript">
~~~{.js}
  var hp = $monster.hp();
  var mana = $monster.mana();
  var name = $monster.name();
~~~
</div>
<div class="language-php">
~~~{.php}
  $hp = $monster->getHp();
  $mana = $monster->getMana();
  $name = monster->getName();
~~~
</div>

These should hold `300`, `150`, and `"Orc"` respectively.

*Note: We never stored a value in `mp`, so we got the default value of `150`.*

To access sub-objects, in the case of our `pos`, which is a `Vec3`:

<div class="language-cpp">
~~~{.cpp}
  auto pos = monster->pos();
  auto x = pos->x();
  auto y = pos->y();
  auto z = pos->z();
~~~
</div>
<div class="language-java">
~~~{.java}
  Vec3 pos = monster.pos();
  float x = pos.x();
  float y = pos.y();
  float z = pos.z();
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  var pos = monster.Pos
  var x = pos.X
  var y = pos.Y
  var z = pos.Z
~~~
</div>
<div class="language-go">
~~~{.go}
  pos := monster.Pos(nil)
  x := pos.X()
  y := pos.Y()
  z := pos.Z()

  // Note: Whenever you access a new object, like in `Pos()`, a new temporary
  // accessor object gets created. If your code is very performance sensitive,
  // you can pass in a pointer to an existing `Vec3` instead of `nil`. This
  // allows you to reuse it across many calls to reduce the amount of object
  // allocation/garbage collection.
~~~
</div>
<div class="language-python">
~~~{.py}
  pos = monster.Pos()
  x = pos.X()
  y = pos.Y()
  z = pos.Z()
~~~
</div>
<div class="language-javascript">
~~~{.js}
  var pos = monster.pos();
  var x = pos.x();
  var y = pos.y();
  var z = pos.z();
~~~
</div>
<div class="language-php">
~~~{.php}
  $pos = $monster->getPos();
  $x = $pos->getX();
  $y = $pos->getY();
  $z = $pos->getZ();
~~~
</div>

`x`, `y`, and `z` will contain `1.0`, `2.0`, and `3.0`, respectively.

*Note: Had we not set `pos` during serialization, it would be `NULL`-value.*

Similarly, we can access elements of the inventory `vector` by indexing it. You
can also iterate over the length of the array/vector representing the
FlatBuffers `vector`.

<div class="language-cpp">
~~~{.cpp}
  auto inv = monster->inventory(); // A pointer to a `flatbuffers::Vector<>`.
  auto inv_len = inv->Length();
  auto third_item = inv->Get(2);
~~~
</div>
<div class="language-java">
~~~{.java}
  int invLength = monster.inventoryLength();
  byte thirdItem = monster.inventory(2);
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  int invLength = monster.InventoryLength;
  var thirdItem = monster.GetInventory(2);
~~~
</div>
<div class="language-go">
~~~{.go}
  invLength := monster.InventoryLength()
  thirdItem := monster.Inventory(2)
~~~
</div>
<div class="language-python">
~~~{.py}
  inv_len = monster.InventoryLength()
  third_item = monster.Inventory(2)
~~~
</div>
<div class="language-javascript">
~~~{.js}
  var invLength = monster.inventoryLength();
  var thirdItem = monster.inventory(2);
~~~
</div>
<div class="language-php">
~~~{.php}
  $inv_len = $monster->getInventoryLength();
  $third_item = $monster->getInventory(2);
~~~
</div>

For `vector`s of `table`s, you can access the elements like any other vector,
except your need to handle the result as a FlatBuffer `table`:

<div class="language-cpp">
~~~{.cpp}
  auto weapons = monster->weapons(); // A pointer to a `flatbuffers::Vector<>`.
  auto weapon_len = weapons->Length();
  auto second_weapon_name = weapons->Get(1)->name()->str();
  auto second_weapon_damage = weapons->Get(1)->damage()
~~~
</div>
<div class="language-java">
~~~{.java}
  int weaponsLength = monster.weaponsLength();
  String secondWeaponName = monster.weapons(1).name();
  short secondWeaponDamage = monster.weapons(1).damage();
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  int weaponsLength = monster.WeaponsLength;
  var secondWeaponName = monster.GetWeapons(1).Name;
  var secondWeaponDamage = monster.GetWeapons(1).Damage;
~~~
</div>
<div class="language-go">
~~~{.go}
  weaponLength := monster.WeaponsLength()
  weapon := new(sample.Weapon) // We need a `sample.Weapon` to pass into `monster.Weapons()`
                               // to capture the output of the function.
  if monster.Weapons(weapon, 1) {
          secondWeaponName := weapon.Name()
          secondWeaponDamage := weapon.Damage()
  }
~~~
</div>
<div class="language-python">
~~~{.py}
  weapons_length = monster.WeaponsLength()
  second_weapon_name = monster.Weapons(1).Name()
  second_weapon_damage = monster.Weapons(1).Damage()
~~~
</div>
<div class="language-javascript">
~~~{.js}
  var weaponsLength = monster.weaponsLength();
  var secondWeaponName = monster.weapons(1).name();
  var secondWeaponDamage = monster.weapons(1).damage();
~~~
</div>
<div class="language-php">
~~~{.php}
  $weapons_len = $monster->getWeaponsLength();
  $second_weapon_name = $monster->getWeapons(1)->getName();
  $second_weapon_damage = $monster->getWeapons(1)->getDamage();
~~~
</div>

Last, we can access our `Equipped` FlatBuffer `union`. Just like when we created
the `union`, we need to get both parts of the `union`: the type and the data.

We can access the type to dynamically cast the data as needed (since the
`union` only stores a FlatBuffer `table`).

<div class="language-cpp">
~~~{.cpp}
  auto union_type = monster.equipped_type();

  if (union_type == Equipment_Weapon) {
    auto weapon = static_cast<const Weapon*>(monster->equipped()); // Requires `static_cast`
                                                                   // to type `const Weapon*`.

    auto weapon_name = weapon->name()->str(); // "Axe"
    auto weapon_damage = weapon->damage();    // 5
  }
~~~
</div>
<div class="language-java">
~~~{.java}
  int unionType = monster.EquippedType();

  if (unionType == Equipment.Weapon) {
    Weapon weapon = (Weapon)monster.equipped(new Weapon()); // Requires explicit cast
                                                            // to `Weapon`.

    String weaponName = weapon.name();    // "Axe"
    short weaponDamage = weapon.damage(); // 5
  }
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  var unionType = monster.EquippedType;

  if (unionType == Equipment.Weapon) {
    var weapon = (Weapon)monster.GetEquipped(new Weapon()); // Requires explicit cast
                                                            // to `Weapon`.

    var weaponName = weapon.Name;     // "Axe"
    var weaponDamage = weapon.Damage; // 5
  }
~~~
</div>
<div class="language-go">
~~~{.go}
  // We need a `flatbuffers.Table` to capture the output of the
  // `monster.Equipped()` function.
  unionTable := new(flatbuffers.Table)

  if monster.Equipped(unionTable) {
          unionType := monster.EquippedType()

          if unionType == sample.EquipmentWeapon {
                  // Create a `sample.Weapon` object that can be initialized with the contents
                  // of the `flatbuffers.Table` (`unionTable`), which was populated by
                  // `monster.Equipped()`.
                  unionWeapon = new(sample.Weapon)
                  unionWeapon.Init(unionTable.Bytes, unionTable.Pos)

                  weaponName = unionWeapon.Name()
                  weaponDamage = unionWeapon.Damage()
          }
  }
~~~
</div>
<div class="language-python">
~~~{.py}
  union_type = monster.EquippedType()

  if union_type == MyGame.Sample.Equipment.Equipment().Weapon:
    # `monster.Equipped()` returns a `flatbuffers.Table`, which can be used to
    # initialize a `MyGame.Sample.Weapon.Weapon()`.
    union_weapon = MyGame.Sample.Weapon.Weapon()
    union_weapon.Init(monster.Equipped().Bytes, monster.Equipped().Pos)

    weapon_name = union_weapon.Name()     // 'Axe'
    weapon_damage = union_weapon.Damage() // 5
~~~
</div>
<div class="language-javascript">
~~~{.js}
  var unionType = monster.equippedType();

  if (unionType == MyGame.Sample.Equipment.Weapon) {
    var weapon_name = monster.equipped(new MyGame.Sample.Weapon()).name();     // 'Axe'
    var weapon_damage = monster.equipped(new MyGame.Sample.Weapon()).damage(); // 5
  }
~~~
</div>
<div class="language-php">
~~~{.php}
  $union_type = $monster->getEquippedType();

  if ($union_type == \MyGame\Sample\Equipment::Weapon) {
    $weapon_name = $monster->getEquipped(new \MyGame\Sample\Weapon())->getName();     // "Axe"
    $weapon_damage = $monster->getEquipped(new \MyGame\Sample\Weapon())->getDamage(); // 5
  }
~~~
</div>

## Mutating FlatBuffers

As you saw above, typically once you have created a FlatBuffer, it is read-only
from that moment on. There are, however, cases where you have just received a
FlatBuffer, and you'd like to modify something about it before sending it on to
another recipient. With the above functionality, you'd have to generate an
entirely new FlatBuffer, while tracking what you modified in your own data
structures. This is inconvenient.

For this reason FlatBuffers can also be mutated in-place. While this is great
for making small fixes to an existing buffer, you generally want to create
buffers from scratch whenever possible, since it is much more efficient and the
API is much more general purpose.

To get non-const accessors, invoke `flatc` with `--gen-mutable`.

Similar to how we read fields using the accessors above, we can now use the
mutators like so:

<div class="language-cpp">
~~~{.cpp}
  auto monster = GetMutableMonster(buffer_pointer);  // non-const
  monster->mutate_hp(10);                      // Set the table `hp` field.
  monster->mutable_pos()->mutate_z(4);         // Set struct field.
  monster->mutable_inventory()->Mutate(0, 1);  // Set vector element.
~~~
</div>
<div class="language-java">
~~~{.java}
  Monster monster = Monster.getRootAsMonster(buf);
  monster.mutateHp(10);            // Set table field.
  monster.pos().mutateZ(4);        // Set struct field.
  monster.mutateInventory(0, 1);   // Set vector element.
~~~
</div>
<div class="language-csharp">
~~~{.cs}
  var monster = Monster.GetRootAsMonster(buf);
  monster.MutateHp(10);            // Set table field.
  monster.Pos.MutateZ(4);          // Set struct field.
  monster.MutateInventory(0, 1);   // Set vector element.
~~~
</div>
<div class="language-go">
~~~{.go}
  <API for mutating FlatBuffers is not yet available in Go.>
~~~
</div>
<div class="language-python">
~~~{.py}
  <API for mutating FlatBuffers is not yet available in Python.>
~~~
</div>
<div class="language-javascript">
~~~{.js}
  <API for mutating FlatBuffers is not yet support in JavaScript.>
~~~
</div>
<div class="language-php">
~~~{.php}
  <API for mutating FlatBuffers is not yet supported in PHP.>
~~~
</div>

We use the somewhat verbose term `mutate` instead of `set` to indicate that this
is a special use case, not to be confused with the default way of constructing
FlatBuffer data.

After the above mutations, you can send on the FlatBuffer to a new recipient
without any further work!

Note that any `mutate` functions on a table will return a boolean, which is
`false` if the field we're trying to set is not present in the buffer. Fields
that are not present if they weren't set, or even if they happen to be equal to
the default value. For example, in the creation code above, the `mana`
field is equal to `150`, which is the default value, so it was never stored in
the buffer. Trying to call the corresponding `mutate` method for `mana` on such
data will return `false`, and the value won't actually be modified!

One way to solve this is to call `ForceDefaults` on a FlatBufferBuilder to
force all fields you set to actually be written. This, of course, increases the
size of the buffer somewhat, but this may be acceptable for a mutable buffer.


## JSON with FlatBuffers

#### Using `flatc` as a Conversion Tool

This is often the preferred method to use JSON with FlatBuffers, as it doesn't
require you to add any new code to your program. It is also efficient, since you
can ship with the binary data. The drawback is that it requires an extra step
for your users/developers to perform (although it may be able to be automated
as part of your compilation).

Lets say you have a JSON file that describes your monster. In this example,
we will use the file `flatbuffers/samples/monsterdata.json`.

Here are the contents of the file:

~~~{.json}
{
  pos: {
    x: 1,
    y: 2,
    z: 3
  },
  hp: 300,
  name: "Orc"
}
~~~

You can run this file through the `flatc` compile with the `-b` flag and
our `monster.fbs` schema to produce a FlatBuffer binary file.

~~~{.sh}
./../flatc -b monster.fbs monsterdata.json
~~~

The output of this will be a file `monsterdata.bin`, which will contain the
FlatBuffer binary representation of the contents from our `.json` file.

<div class="language-cpp">
*Note: If you're working in C++, you can also parse JSON at runtime. See the
[Use in C++](@ref flatbuffers_guide_use_cpp) section of the Programmer's
Guide for more information.*
</div>

## Advanced Features for Each Language

Each language has a dedicated `Use in XXX` page in the Programmer's Guide
to cover the nuances of FlatBuffers in that language.

For your chosen language, see:

<div class="language-cpp">
[Use in C++](@ref flatbuffers_guide_use_cpp)
</div>
<div class="language-java">
[Use in Java/C#](@ref flatbuffers_guide_use_java_c-sharp)
</div>
<div class="language-csharp">
[Use in Java/C#](@ref flatbuffers_guide_use_java_c-sharp)
</div>
<div class="language-go">
[Use in Go](@ref flatbuffers_guide_use_go)
</div>
<div class="language-python">
[Use in Python](@ref flatbuffers_guide_use_python)
</div>
<div class="language-javascript">
[Use in JavaScript](@ref flatbuffers_guide_use_javascript)
</div>
<div class="language-php">
[Use in PHP](@ref flatbuffers_guide_use_php)
</div>

<br>