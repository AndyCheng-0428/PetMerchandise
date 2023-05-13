using System;
using System.Net;
using System.Text;
using PetMerchandise.core.constant;

namespace PetMerchandise.core.view_model.web;

public class WebViewModel : BaseViewModel
{
    private HttpListener _httpListener;

    public WebViewModel()
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(FacebookApplicationConstant.REDIRECT_URL);
        _httpListener.Start();
        _httpListener.BeginGetContext(HandleRequest, _httpListener);
    }

    private void HandleRequest(IAsyncResult ar)
    {
        var context = _httpListener.EndGetContext(ar);
        // 創建Http響應
        var response = context.Response;
        response.ContentType = "text/html";
        
        // 構建HTML文檔
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("<html>");
        stringBuilder.Append("  <body>");
        stringBuilder.Append("      <h1>Hello, World!</h1>");
        stringBuilder.Append("  </body>");
        stringBuilder.Append("</html>");
        var buffer = Encoding.UTF8.GetBytes(stringBuilder.ToString());
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
        _httpListener.BeginGetContext(HandleRequest, _httpListener);
    }
}