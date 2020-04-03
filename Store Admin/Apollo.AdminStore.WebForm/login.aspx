<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Apollo.AdminStore.WebForm.login" %>
<!DOCTYPE html>
<html>

<head>    
    <title>Apollo WebAdmin</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="/css/inspinia/bootstrap.min.css" rel="stylesheet">
    <link href="/css/inspinia/font-awesome.css" rel="stylesheet">
    <link href="/css/inspinia/animate.css" rel="stylesheet">
    <link href="/css/inspinia/style.css" rel="stylesheet">
    <!-- Global site tag (gtag.js) - Google Analytics -->
    <script async src="https://www.googletagmanager.com/gtag/js?id=UA-4319261-6"></script>
    <script>
      window.dataLayer = window.dataLayer || [];
      function gtag(){dataLayer.push(arguments);}
      gtag('js', new Date());

      gtag('config', 'UA-4319261-6');
    </script>
</head>

<body class="gray-bg">
    <div class="middle-box text-center loginscreen animated fadeInDown">
        <div>
            <div><h1 class="logo-name">AP</h1></div>
            <h3>Welcome to Apollo WebAdmin</h3>
            <asp:Literal ID="ltMessage" runat="server"></asp:Literal>
            <form class="m-t" role="form" method="post" action="login.aspx">
                <div class="form-group">
                    <input type="email" class="form-control" placeholder="Username" name="username" required="">
                </div>
                <div class="form-group">
                    <input type="password" class="form-control" placeholder="Password" name="password" required="">
                </div>
                <button type="submit" class="btn btn-primary block full-width m-b">Login</button>                
            </form>
            <p class="m-t"> <small>Apollo &copy; 2008-<%= DateTime.Now.Year %></small> </p>
            <p class="m-t"> <small>version <%= typeof(Apollo.AdminStore.WebForm.Global).Assembly.GetName().Version.ToString() %></small> </p>
        </div>
    </div>

    <!-- Mainly scripts -->
    <script src="/js/inspinia/jquery-2.1.1.js"></script>
    <script src="/js/inspinia/bootstrap.min.js"></script>

</body>

</html>
