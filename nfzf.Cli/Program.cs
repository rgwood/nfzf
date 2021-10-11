using Terminal.Gui;
using nfzf;

// todo: change this based on terminal size?
const int MaxResultsAtOnce = 20;

Application.Init();
var top = Application.Top;

List<string> allRows = cannedRows();

var textField = new TextField("")
{
    X = 0,
    Y = Pos.Bottom(top) - 1,
    Width = Dim.Fill()
};

var list = new ListView(allRows.Take(MaxResultsAtOnce).ToList());
list.Height = Dim.Fill(1);
list.X = 0;
list.Y = 0;
list.Width = Dim.Fill();

textField.TextChanged += (oldText) =>
{
    string? query = textField.Text.ToString();

    if (string.IsNullOrEmpty(query))
    {
        list.SetSource(allRows.Take(MaxResultsAtOnce).ToList());
    }
    else
    {
        var filtered = from row in allRows
                       let score = Algo.FuzzyMatchV1(false, false, true, row, query, false).Result.Score
                       where score > 0
                       orderby score descending
                       select row;

        list.SetSource(filtered.Take(MaxResultsAtOnce).ToList());
    }
    
};

top.Add(
    list,
    textField);

Application.Run();



List<string> cannedRows() => new List<string>() {
"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
"Viverra aliquet eget sit amet tellus cras adipiscing enim eu.",
"Cursus metus aliquam eleifend mi.",
"Ipsum dolor sit amet consectetur adipiscing elit pellentesque habitant morbi.",
"Aliquam malesuada bibendum arcu vitae elementum curabitur vitae nunc.",
"Dictumst vestibulum rhoncus est pellentesque elit ullamcorper.",
"Amet justo donec enim diam vulputate ut.",
"Est ullamcorper eget nulla facilisi etiam dignissim diam quis enim.",
"Est sit amet facilisis magna etiam tempor orci.",
"Dolor purus non enim praesent.",
"Lectus mauris ultrices eros in cursus turpis massa tincidunt dui.",
"Aliquet bibendum enim facilisis gravida.",
"Est pellentesque elit ullamcorper dignissim cras tincidunt lobortis feugiat.",
"Lobortis scelerisque fermentum dui faucibus.",
"Erat pellentesque adipiscing commodo elit.",
"Ultricies leo integer malesuada nunc vel risus.",
"A cras semper auctor neque.",
"Viverra nibh cras pulvinar mattis nunc.",
"Quis auctor elit sed vulputate mi sit amet mauris commodo.",
"Dictumst vestibulum rhoncus est pellentesque.",
"Lectus vestibulum mattis ullamcorper velit sed ullamcorper morbi tincidunt ornare.",
"Sed egestas egestas fringilla phasellus faucibus scelerisque.",
"Id eu nisl nunc mi ipsum faucibus vitae aliquet.",
"Adipiscing elit duis tristique sollicitudin nibh sit amet commodo nulla.",
"Amet consectetur adipiscing elit duis tristique sollicitudin nibh sit.",
"Sed vulputate mi sit amet mauris commodo quis imperdiet massa.",
"Sapien nec sagittis aliquam malesuada bibendum arcu vitae elementum curabitur.",
"Et odio pellentesque diam volutpat commodo sed egestas egestas.",
"Dolor morbi non arcu risus quis varius quam.",
"Vel elit scelerisque mauris pellentesque pulvinar pellentesque habitant morbi tristique.",
"Adipiscing diam donec adipiscing tristique risus nec feugiat in fermentum.",
"Justo eget magna fermentum iaculis.",
"Nisl nisi scelerisque eu ultrices vitae auctor eu augue.",
"Sagittis eu volutpat odio facilisis mauris.",
"Sagittis id consectetur purus ut faucibus pulvinar elementum.",
"Etiam tempor orci eu lobortis elementum nibh tellus molestie nunc.",
"Cras sed felis eget velit aliquet sagittis id consectetur purus.",
"Ut tellus elementum sagittis vitae.",
"Nec nam aliquam sem et tortor.",
"Eu feugiat pretium nibh ipsum consequat nisl vel.",
"Ornare arcu dui vivamus arcu felis bibendum ut tristique.",
"Ultrices vitae auctor eu augue ut.",
"Volutpat ac tincidunt vitae semper quis lectus.",
"Vitae congue eu consequat ac felis donec et odio pellentesque.",
"Tortor id aliquet lectus proin nibh nisl.",
"Non odio euismod lacinia at quis risus.",
"At auctor urna nunc id cursus metus.",
"Tortor condimentum lacinia quis vel eros donec ac odio.",
"Mauris sit amet massa vitae tortor condimentum lacinia quis.",
"Diam donec adipiscing tristique risus nec feugiat in fermentum posuere.",
"Quis vel eros donec ac odio tempor orci.",
"Vitae auctor eu augue ut lectus arcu.",
"At lectus urna duis convallis convallis tellus.",
"Tempus quam pellentesque nec nam aliquam sem et tortor.",
"Nunc vel risus commodo viverra maecenas accumsan lacus vel facilisis.",
"Et tortor consequat id porta nibh venenatis cras sed.",
"Purus ut faucibus pulvinar elementum integer enim neque.",
"Consequat mauris nunc congue nisi vitae suscipit tellus mauris.",
"Nisi lacus sed viverra tellus in hac habitasse.",
"Nulla aliquet enim tortor at auctor urna nunc id cursus.",
"At in tellus integer feugiat scelerisque.",
"Rhoncus est pellentesque elit ullamcorper dignissim cras tincidunt lobortis feugiat.",
"Vel elit scelerisque mauris pellentesque pulvinar pellentesque habitant morbi tristique.",
"Eget duis at tellus at urna.",
"Amet luctus venenatis lectus magna fringilla urna porttitor rhoncus.",
"Nunc consequat interdum varius sit amet mattis.",
"Consequat interdum varius sit amet mattis vulputate enim.",
"Viverra aliquet eget sit amet tellus cras adipiscing.",
"Ipsum dolor sit amet consectetur adipiscing elit.",
"Leo vel fringilla est ullamcorper eget nulla facilisi.",
"Praesent semper feugiat nibh sed.",
"Commodo nulla facilisi nullam vehicula.",
"Tortor consequat id porta nibh venenatis cras sed.",
"Volutpat blandit aliquam etiam erat."
};