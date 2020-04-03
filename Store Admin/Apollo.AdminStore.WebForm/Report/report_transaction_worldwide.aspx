<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="report_transaction_worldwide.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Report.report_transaction_worldwide" %>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Jvectormap -->
    <script src="/js/inspinia/plugins/jvectormap/jquery-jvectormap-2.0.2.min.js"></script>
    <script src="/js/inspinia/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"></script>

    <script>
        $(document).ready(function () {
            var mapData = {
                "US": 298,
                "SA": 200,
                "DE": 220,
                "FR": 540,
                "CN": 120,
                "AU": 760,
                "BR": 550,
                "IN": 200,
                "GB": 120,
            };

            $('#world-map').vectorMap({
                map: 'world_mill_en',
                backgroundColor: "transparent",
                regionStyle: {
                    initial: {
                        fill: '#e4e4e4',
                        "fill-opacity": 0.9,
                        stroke: 'none',
                        "stroke-width": 0,
                        "stroke-opacity": 0
                    }
                },

                series: {
                    regions: [{
                        values: mapData,
                        scale: ["#1ab394", "#22d6b1"],
                        normalizeFunction: 'polynomial'
                    }]
                },
            });
        });
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Report</h2>
            <h3>Transaction Worldwide</h3>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">                
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="row">
                            <div class="col-lg-6">
                                <table class="table table-hover margin bottom">
                                    <thead>
                                        <tr>
                                            <th style="width: 1%" class="text-center">Country</th>
                                            <th>Transaction</th>
                                            <th class="text-center">Number of Orders</th>
                                            <th class="text-center">Amount</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td class="text-center">1</td>
                                            <td> United Kingdom</td>
                                            <td class="text-center small">100</td>
                                            <td class="text-center"><span class="label label-primary">$483.00</span></td>
                                        </tr>
                                        <tr>
                                            <td class="text-center">2</td>
                                            <td> United States</td>
                                            <td class="text-center small">20</td>
                                            <td class="text-center"><span class="label label-primary">$327.00</span></td>
                                        </tr>
                                        <tr>
                                            <td class="text-center">3</td>
                                            <td> Australia</td>
                                            <td class="text-center small">12</td>
                                            <td class="text-center"><span class="label label-warning">$125.00</span></td>
                                        </tr>                                    
                                    </tbody>
                                </table>
                            </div>
                            <div class="col-lg-6">
                                <div id="world-map" style="height: 300px;"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>