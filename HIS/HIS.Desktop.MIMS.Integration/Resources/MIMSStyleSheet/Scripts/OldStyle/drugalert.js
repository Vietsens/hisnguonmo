function sortObjects(arrayName,length,desc) {
    for (var i=0; i<(length-1); i++)
        for (var j=i+1; j<length; j++)
            // Descending reason
            if (desc==1) {
                if (arrayName[j].id > arrayName[i].id) {
                    var dummy = arrayName[i];
                    arrayName[i] = arrayName[j];
                    arrayName[j] = dummy;
                }
            } else {
                if (arrayName[j].id < arrayName[i].id) {
                    var dummy = arrayName[i];
                    arrayName[i] = arrayName[j];
                    arrayName[j] = dummy;
                }
            }
}

$.fn.reorder = function(desc) {
  
  return($(this).each(function() {
    var $this = $(this);
    var $children = $this.children();
    var childCount = $children.length;

    if (childCount > 1) {
        $children.hide();

        sortObjects($children, childCount, desc);
        
        // After sorting, the accordion object is not updated correctly.
        // Workaround is to clone (a deep copy) and insert as new. Then remove the object.
        for (var i=0; i<childCount; i++)
        {
            var $child = $children.eq(i);
            var $clone = $child.clone(true);
            $clone.show().appendTo($this);
            
            $child.remove();
        }        
    }
  }));
}

$(document).ready(
        function() {
            $("#list_interactions").accordion({ header: "h3", active: "false", collapsible: "true", fillSpace: "true" });
            $("#list_allergies").accordion({ header: "h3", active: "false", collapsible: "true" });
            $("#list_health").accordion({ header: "h3", active: "false", collapsible: "true", fillSpace: "true" });
			$("#list_vn").accordion({ header: "h3", active: "false", collapsible: "true", fillSpace: "true" });
//Customizing the Drug Therapy and Drug Ingradients
			$("#list_dup").accordion({header: "h3",active: "false",collapsible: "false",fillSpace: "true",icons:null});
			$("#list_dup.ui-accordion-header").css("color","black") ;
			//$("#list_dup .ui-state-active").css("color", "#000000");
			$("#list_duping").accordion({header: "h3",active: "false",collapsible: "false",fillSpace: "true",icons:null});         
		 //  $("#list_dup").accordion({ header: "h3", active: "false", collapsible: "true" });
		//	$("#list_duping").accordion({ header: "h3", active: "false", collapsible: "true" });
            $("#list_dose").accordion({ header: "h3", active: "false", collapsible: "true", fillSpace: "true" });
            $("#list_preg").accordion({ header: "h3", active: "false", collapsible: "true", fillSpace: "true" });
            $("#list_lact").accordion({ header: "h3", active: "false", collapsible: "true", fillSpace: "true" });

            $('#tabs').tabs();  // Do this last, otherwise the allergy accordion will be messed up

            $("#list_interactions").reorder(1);
            $("#list_health").reorder(1);
            $("#list_allergies").reorder(0);
            $("#list_dup").reorder(1);
			//applied for DI sorting
	    $("#list_duping").reorder(1)
            $("#list_dose").reorder(0);
            $("#list_preg").reorder(1);
            $("#list_lact").reorder(0);

            // Convert all button to jQuery buttons
            $("button").button({icons: { secondary: 'ui-icon-arrowthickstop-1-s'} });
            $("button").click(function(e) {
                e.preventDefault();
                var target = '#' + e.currentTarget.id.replace(/^_b/gi, "");
                $(this).hide();
                $(target).toggle();
            });
        }
    );

