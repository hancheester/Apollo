<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderSystemCheckViewControl.ascx.cs" Inherits="Apollo.AdminStore.WebForm.UserControls.OrderSystemCheckViewControl" %>
<div class="panel panel-default">
    <div class="panel-heading">
        System Check scores
    </div>
    <div id='scores_<%= OrderId %>'>
        <table class="table">
            <tr>
                <th>Scores</th>
                <td>
                    <a href="javascript:void(0);" onclick="javascript:showSystemCheckScore(this, <%= OrderId  %>);">
                        <i class="pSystemScore fa fa-question-circle"></i>
                    </a>
                </td>
            </tr>
        </table>
    </div>
    <table class="table">
        <tr><th colspan="2">Guideline</th></tr>
        <tr>
            <td>&lt; 150</td>
            <td>Should be okay to send</td>
        </tr>
        <tr>
            <td>150 - 250</td>
            <td>Should ask for confirmation</td>
        </tr>
        <tr>
            <td>&gt; 250</td>
            <td>Should not send it</td>
        </tr>
    </table>
</div>

<script type="text/javascript">   
    function showSystemCheckScore(sender, orderid) {
        $(sender).find('.pSystemScore').attr('class', 'fa fa-spinner fa-spin');        
        <%= this.Page.ClientScript.GetCallbackEventReference(this, "orderid", "loadingScores", "orderid") %>;
    }
    
    function loadingScores(msg, context) {
        $('#scores_' + context).html(msg);
    }    
</script>

<asp:PlaceHolder ID="phTrigger" runat="server" Visible="false">
    <script type="text/javascript">
        function load() {
            var orderid = <%= OrderId %>;
            var el = document.getElementsByClassName('pSystemScore');
            el[0].className = 'fa fa-spinner fa-spin';
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "orderid", "loadingScores", "orderid") %>;
            clearInterval(runner);
        }

        var runner = setInterval(load, 1000);
    </script>
</asp:PlaceHolder>