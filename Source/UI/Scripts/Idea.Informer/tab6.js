var reformal_wdg_domain = "nemestats";
function ref_ud(a){document.write(a);}
function ref_id(id){return document.getElementById(id);}

var dref_mode = (typeof reformal_wdg_mode != "undefined") ? reformal_wdg_mode : 0 ;
var dref_title = (typeof reformal_wdg_title != "undefined") ? reformal_wdg_title : "Feedback" ;
var dref_ltitle = (typeof reformal_wdg_ltitle != "undefined") ? reformal_wdg_ltitle : "Add feedback" ;
var dref_lfont = (typeof reformal_wdg_lfont != "undefined") ? reformal_wdg_lfont : "" ;
var dref_lsize = (typeof reformal_wdg_lsize != "undefined") ? reformal_wdg_lsize : "12px" ;
var dref_color = (typeof reformal_wdg_color != "undefined" && reformal_wdg_color!='#') ? reformal_wdg_color : "#FFA000" ;
var dref_align = (typeof reformal_wdg_align != "undefined" && reformal_wdg_align!='') ? reformal_wdg_align : "left" ;
var dref_charset = (typeof reformal_wdg_charset != "undefined") ? reformal_wdg_charset : "" ;
var dref_waction = (typeof reformal_wdg_waction != "undefined") ? reformal_wdg_waction : 0 ;

var dref_ext_cms = ((typeof reformal_wdg_cms != "undefined") ? reformal_wdg_cms : 'reformal') ;

var dref_ext_img = (typeof reformal_wdg_bimage != "undefined" && reformal_wdg_bimage!='') ? 1 : 0 ;
var dref_ext_img_m = (dref_ext_img && reformal_wdg_bimage.substring(3, reformal_wdg_bimage).toLowerCase()=='htt') ? 1 : 0;
if (dref_ext_img_m && reformal_wdg_bimage.indexOf( 'idea.informer.com/files/', 0 ) > 0) { dref_ext_img_m = 0; var v = reformal_wdg_bimage.toString().split ( '/' ); reformal_wdg_bimage = v[v.length-1]; }


	
out_link = 'http://'+reformal_wdg_domain+'.idea.informer.com/proj/?mod=one';


if (dref_waction) 
{
		vlink = (typeof reformal_wdg_vlink != "undefined")  ? 'http://'+reformal_wdg_vlink : 'http://'+reformal_wdg_domain+'.idea.informer.com/proj/?mod=one';
}
else
{
		vlink = 'javascript:MyOtziv.mo_show_box();';
}

var vsiteAdr = (typeof reformal_wdg_vlink != "undefined")? 1 : 0;

MyOtzivCl = function() {
    var siteAdr = 'http://idea.informer.com/';
    
    this.mo_get_win_width = function() {
        var myWidth = 0;
        if( typeof( window.innerWidth ) == 'number' )             myWidth = window.innerWidth;
        else if( document.documentElement && document.documentElement.clientWidth )             myWidth = document.documentElement.clientWidth;
        else if( document.body && document.body.clientWidth)             myWidth = document.body.clientWidth;
        return myWidth;
    }
	
    this.mo_get_win_height = function() {
        var myHeight = 0;
        if( typeof( window.innerHeight ) == 'number' )             myHeight = window.innerHeight;
        else if( document.documentElement && document.documentElement.clientHeight )             myHeight = document.documentElement.clientHeight;
        else if( document.body && document.body.clientHeight)             myHeight = document.body.clientHeight;
        return myHeight;
    }

    this.mo_get_scrol = function() {
        var yPos = 0;
        if (self.pageYOffset) {
            yPos = self.pageYOffset;
        } else if (document.documentElement && document.documentElement.scrollTop){
            yPos = document.documentElement.scrollTop;
        } else if (document.body) {
            yPos = document.body.scrollTop;
        }
        return yPos;
    }

    this.mo_show_box = function() {
	    if (document.getElementById("fthere").innerHTML == "") {
		    document.getElementById("fthere").innerHTML = "<iframe src=\""+(vsiteAdr?'http://'+reformal_wdg_vlink+'/':siteAdr)+"wdg1v1.php?domain="+reformal_wdg_domain+"\" width=\"627\" height=\"250\" align=\"left\" style=\"border: 0; position:relative;\" frameborder=\"0\" scrolling=\"no\">Frame error</iframe>";
		}
//        var l = this.mo_get_win_width()/2 - 350;
//        var t = this.mo_get_win_height()/2 - 200 + this.mo_get_scrol();
//        document.getElementById('myotziv_box').style.top  = (dref_ext_cms=='joomla') ? '35px' : t+'px';
//        document.getElementById('myotziv_box').style.left = l+'px'; 
        document.getElementById('myotziv_box').style.display='block';
    }

    this.mo_hide_box = function() {
        document.getElementById('myotziv_box').style.display='none';
    }
    
    this.mo_showcss = function() {
        ref_ud("<style type=\"text/css\">");
        
        ref_ud(".tdsfh{background: url('"+(dref_ext_img ? (dref_ext_img_m ? reformal_wdg_bimage : siteAdr+'files/images/buttons/'+reformal_wdg_bimage) : siteAdr+'i/feedback_tab.png' )+"');} * html .tdsfh{background-image: none; filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"+(dref_ext_img ? (dref_ext_img_m ? reformal_wdg_bimage : siteAdr+'files/images/buttons/'+reformal_wdg_bimage) : siteAdr+'i/feedback_tab.png' )+"');} .widsnjx {margin:0 auto; "+((dref_ext_cms=='joomla') ? 'position:static;' : 'position:relative;')+" } .widsnjx fieldset {padding:0; border:none; border:0px solid #000; margin:0;} .furjbqy { position:fixed; left:0; top:0; z-index:5; width:22px; height:100%; } * html .furjbqy { position:absolute; } .furjbqy table {border-collapse: collapse;} .furjbqy td {padding: 0px; vertical-align: middle;} .furjbqy a { display:block; background:"+(dref_ext_img_m ? 'none' :dref_color)+"; } .furjbqy a:hover { background:"+(dref_ext_img_m ? 'none' :dref_color)+"; }.furjbqy img {border:0;}");
        ref_ud(".furrghtd { position:fixed; right:0px; top:0; z-index:5; width:22px; height:100%; } * html .furrghtd { position:absolute; } .furrghtd table {border-collapse: collapse;} .furrghtd td {padding: 0px; vertical-align: middle;} .furrghtd a { display:block; background:"+(dref_ext_img_m ? 'none' :dref_color)+"; } .furrghtd a:hover { background:"+(dref_ext_img_m ? 'none' :dref_color)+"; }.furrghtd img {border:0;} #poxupih {position:absolute; z-index:1001; width:689px;  top:0px; left:0px; font-size:11px; color:#3F4543; font-family: \"Segoe UI\", Arial, Tahoma, sans-serif;}.poxupih_top {width:689px; height:28px; background:transparent url("+siteAdr+"tmpl/images/popup_idea_top.png) 0 0 no-repeat;}.poxupih_bt {width:689px; height:28px; background:transparent url("+siteAdr+"tmpl/images/popup_idea_bt.png) 0 0 no-repeat;}");
        ref_ud(".poxupih_center {width:689px; background:transparent url("+siteAdr+"tmpl/images/popup_idea_bg.png) 0 0 repeat-y;}.poxupih1 {margin: 0 20px; overflow:hidden; background:#efefef; padding:0px;}.fdsrrel {float:right; margin:-2px 5px 0 0;}.bvnmrte {padding: 15px 20px 20px 12px; _padding-left:1px; font-family: \"Segoe UI\", Arial, Tahoma, sans-serif; font-size:11px; color:#3F4543; }.poxupih1 .bvnmrte {padding-bottom:10px; padding-top:0px; background:none;}.gertuik {padding:0 8px 0 20px;}");
        ref_ud("#poxupih #hretge {margin:8px 0px; height:96px; background: #fba11f url("+siteAdr+"tmpl/images/search_bg.gif) 0 0px no-repeat; position:relative;}#hretge form {padding: 10px 19px 0 18px;}#poxupih #bulbulh {width:462px; float:left;}#adihet {float:right;background: transparent url("+siteAdr+"tmpl/images/add_idea_go.gif) 0 0px no-repeat; border:none medium; width:132px; height:27px; float:right; margin-right:-3px; cursor:pointer;}");
        ref_ud("#adihet:hover {background-position: 0 -27px;}.drop_right {background: transparent url("+siteAdr+"tmpl/images/q_right1.gif) 0% 0px no-repeat; float:right; display:block; width:8px; height:11px; margin-top:1px; font-size:0;}.drop_left {background: transparent url("+siteAdr+"tmpl/images/q_left1.gif) 0% 0px no-repeat; float:right; display:block; width:8px; height:11px; margin-top:1px;}.status_right {left:15px !important;  text-align:left; float:right; margin:0 -15px 0 0;}");
        ref_ud("#poxupih  a {position:relative; z-index:10;}#poxupih .idea_green_top {height:1%;}.poxupih_top {_background-image:none; _filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"+siteAdr+"tmpl/images/popup_idea_top.png');}.poxupih_bt {_background-image:none; _filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"+siteAdr+"tmpl/images/popup_idea_bt.png');}.poxupih_center {_background-image:none; _filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"+siteAdr+"tmpl/images/popup_idea_bg.png',sizingmethod='scale');}");

        ref_ud(".adihet_div {float:right;background: url("+siteAdr+"i/add_idea_go_lt.gif) top left no-repeat; height:27px; margin-left:0px; cursor:pointer;}");
        ref_ud(".adihet_div:hover {background-position:0 -27px;}");
        ref_ud("#adihet{background: url("+siteAdr+"i/add_idea_go_rt.gif) top right no-repeat; border:none; height:27px; cursor:pointer; color:#fff;font: 14px/27px 'Segoe UI', Arial, Tahoma, sans-serif;padding:0 8px 3px; margin:0;}");
        ref_ud("#adihet:hover {background-position:100% -27px;}");

        ref_ud("a.pokusijy {display:block; width:16px; height:16px; background: transparent url("+siteAdr+"tmpl/images/cancel.gif) 100% 0px no-repeat; float:right; position:relative; z-index:101;}a.pokusijy:hover {background-position: 100% 100%; cursor:pointer;}.i_prop {font-size:18px; color:#fff; padding: 0 0 5px 0;}#bulbulh {width:600px; padding: 2px 4px; color:#3F4543; font-family: \"Segoe UI\", Arial; font-size:16px; margin-bottom:5px;}#hdsfjfsr {background: transparent url("+siteAdr+"tmpl/images/search_go.gif) 0 0px no-repeat; border:none medium; width:97px; height:27px; float:right; margin-right:-3px; cursor:pointer;}#hdsfjfsr:hover {background-position: 0 -27px;} #poxupih .fdsrrel a {z-index:0;}</style>");
    }
    
    this.mo_showframe = function() {
        this.mo_showcss();
        
        if (!dref_mode) {
            if ('left' == dref_align) {
                if (!dref_ext_img) {
                    ref_ud("<div class=\"furjbqy\"><table height=\"100%\"><tr><td valign=\"middle\"><a id=\"js-suggest-link\" href=\""+vlink+"\""+((dref_waction)?' target=\"_blank\"':"")+"><img src=\""+siteAdr+"tmpl/images/transp.gif\" alt=\"\" style=\"border: 0;\" width=\"22\" height=\"131\" class=\"tdsfh\" /></a></td></tr></table></div>");
                } else { 
                    ref_ud("<div class=\"furjbqy\"><table height=\"100%\"><tr><td valign=\"middle\"><a id=\"js-suggest-link\" href=\"" + vlink + "\"" + ((dref_waction) ? ' target=\"_blank\"' : "") + "><img src=\"" + (dref_ext_img_m ? reformal_wdg_bimage : siteAdr + 'files/images/buttons/' + reformal_wdg_bimage) + "\" alt=\"\" style=\"border: 0;\" class=\"tdsfh\" /></a></td></tr></table></div>");
                }
            } else {
                if (!dref_ext_img)
                {
                    ref_ud("<div class=\"furrghtd\"><table height=\"100%\"><tr><td valign=\"middle\"><a id=\"js-suggest-link\" href=\"" + vlink + "\"" + ((dref_waction) ? ' target=\"_blank\"' : "") + "><img src=\"" + siteAdr + "tmpl/images/transp.gif\" alt=\"\" style=\"border: 0;\" width=\"22\" height=\"131\" class=\"tdsfh\" /></a></td></tr></table></div>");
                }
                else
                {
                    ref_ud("<div class=\"furrghtd\"><table height=\"100%\"><tr><td valign=\"middle\"><a id=\"js-suggest-link\" href=\"" + vlink + "\"" + ((dref_waction) ? ' target=\"_blank\"' : "") + "><img src=\"" + (dref_ext_img_m ? reformal_wdg_bimage : siteAdr + 'files/images/buttons/' + reformal_wdg_bimage) + "\" alt=\"\" style=\"border: 0;\" class=\"tdsfh\" /></a></td></tr></table></div>");
                }
            }
       } else {
            ref_ud("<a id=\"js-suggest-link\" href=\"" + vlink + "\" style=\"color:" + dref_color + "; " + (dref_lfont ? 'font-family:' + dref_lfont + ';' : '') + " " + (dref_lsize ? 'font-size:' + dref_lsize + ';' : '') + "\"" + ((dref_waction) ? ' target=\"_blank\"' : "") + ">" + dref_ltitle + "</a>");
       } 	
		
	    ref_ud("<div style=\"position: absolute; top: 50%; left: 50%;\">");
    	ref_ud("<div style=\""+((dref_ext_cms=='joomla') ? 'position:fixed;' : 'position:absolute;')+" display: none; top: -200px; left: -350px;\" id=\"myotziv_box\">");
        ref_ud("<div class=\"widsnjx\"><div id=\"poxupih\"><div class=\"poxupih_top\"></div><div class=\"poxupih_center\"><div class=\"poxupih1\">");
        ref_ud("<div class=\"gertuik\"><a class=\"pokusijy\" onClick=\"MyOtziv.mo_hide_box();\"></a>");
        ref_ud("<div class=\"fdsrrel\"><a href=\""+siteAdr+"\" target=\"_blank\"><img src=\""+siteAdr+"tmpl/images/widget_logo.jpg\" width=\"109\" height=\"23\" alt=\"\" border=\"0\" /></a></div>"+dref_title+"</div>");
        ref_ud("<div id=\"hretge\"><form target=\"_blank\" action=\""+(vsiteAdr?"http://"+reformal_wdg_vlink+"/proj/":"http://"+reformal_wdg_domain+".idea.informer.com/proj/")+"\" method=\"get\"><input type=\"hidden\" name=\"charset\" value=\""+dref_charset+"\" /><fieldset><div class=\"i_prop\">I would like to...</div><input id=\"bulbulh\" name=\"idea\" type=\"text\" /><div class=\"adihet_div\"><input id=\"adihet\" type=\"submit\" value=\"Add feedback\"></div></fieldset></form></div>");
		
		ref_ud("<div class=\"bvnmrte\" style=\"height: 250px;\" id=\"fthere\"></div>");
		
		ref_ud("</div></div><div class=\"poxupih_bt\"></div></div></div>");        
        
        ref_ud("</div></div>");
    }
}
var MyOtziv = new MyOtzivCl();	
MyOtziv.mo_showframe();