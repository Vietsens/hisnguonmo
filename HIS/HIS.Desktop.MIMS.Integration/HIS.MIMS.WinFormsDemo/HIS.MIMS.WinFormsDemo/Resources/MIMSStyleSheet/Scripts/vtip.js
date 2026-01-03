/**
Vertigo Tip by www.vertigo-project.com
Requires (kind of) jQuery UI, and (definately) jQuery
*/

this.vtip = function() {
    xOffset = 5; // x distance from mouse
    yOffset = 20; // y distance from mouse       

    $(".vtip").hover(

        function(e) {
            this.t = this.title;
            this.title = ''; //stop the actual title showing

            $('body').append( '

' + this.t + '

' );
            $('p#vtip').css("display", "none") //hide it
                       .css("position", "absolute").css("opacity", "0.9").css("padding", "10px") //style
                       .css("top", (e.pageY + yOffset) + "px").css("left", (e.pageX + xOffset) + "px") //position
                       .fadeIn("fast"); //show it
        },
        function() {
            this.title = this.t; // set the title back

            $("p#vtip").remove();
        }

    ).mousemove(

        function(e) {
            $("p#vtip").css("top",(e.pageY - xOffset) + "px")
                       .css("left",(e.pageX + yOffset) + "px");
        }

    );            

};
