using System;
using NUnit.Framework;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace MercadoLibre.SDK.Test
{
	[TestFixture]
	public class MeliTest
	{

        long UserId = 1882558883194856;
        string secretKey = "ugwj2fgeVVT51sGtzgJWTZhqm0mfRtbZ";
        string token = "APP_USR-1882558883194856-082212-bf7c4d00df9a5bcefefd98a9e5cce843-83964105";

        [Test]
		public void GetAuthUrl ()
		{
			Meli m = new Meli (UserId, secretKey);

            string url1 = Meli.AuthUrls.MLA + "/authorization?response_type=code&client_id=" + UserId.ToString() + "&redirect_uri=http%3a%2f%2flocalhost";

            string url2 = m.GetAuthUrl(Meli.AuthUrls.MLA, "http://localhost");

            Assert.AreEqual(url1, url2);
		}

		[Test]
		public void AuthorizationSuccess ()
		{
			
			Meli m = new Meli (UserId, secretKey);

            /****** Esta es la parte que hay que automatizar ***********************/

            Process cho=  Process.Start(@m.GetAuthUrl(Meli.AuthUrls.MLA, "http://localhost"));
            
            /************************************************************************/

            m.Authorize ("TG-5d5abd557cec1f0007274d39-83964105", "http://localhost");
            string accessToken = m.AccessToken;
            string refreshToken = m.RefreshToken;


            Assert.AreEqual ("APP_USR-1882558883194856-082216-2a0338f0714dcb647628f1100d3f75f9-83964105", m.AccessToken);
			Assert.AreEqual ("TG-5b7dc95de4b0e28ca3de95e9-83964105", m.RefreshToken);
		}

		[Test]
		[ExpectedException(typeof(AuthorizationException))]
		public void AuthorizationFailure ()
		{
			Meli.ApiUrl = "http://localhost:3000";

			Meli m = new Meli (123456, "secret client");
			m.Authorize ("invalid code", "http://someurl.com");
		}

		[Test]
		public void Get ()
		{
			Meli.ApiUrl = "http://localhost:3000";

			Meli m = new Meli (UserId, secretKey, token);

			var response = m.Get ("/sites");

			Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);
			Assert.IsNotNullOrEmpty (response.Content);
		}

		[Test]
		public void GetWithRefreshToken ()
		{
			Meli.ApiUrl = "http://localhost:3000";

			Meli m = new Meli (123456, "client secret", "expired token", "valid refresh token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;

			var ps = new List<Parameter> ();
			ps.Add (p);
			var response = m.Get ("/users/me", ps);

			Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);
			Assert.IsNotNullOrEmpty (response.Content);
		}

		[Test]
		public void HandleErrors ()
		{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "invalid token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;
			var ps = new List<Parameter> ();
			ps.Add (p);
			var response = m.Get ("/users/me", ps);
			Assert.AreEqual (HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Test]
		public void Post ()
		{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "valid token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;

			var ps = new List<Parameter> ();
			ps.Add (p);
			var r = m.Post ("/items", ps, new {foo="bar"});

			Assert.AreEqual (HttpStatusCode.Created, r.StatusCode);
		}

		[Test]
		public void PostWithRefreshToken ()
		{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "expired token", "valid refresh token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;

			var ps = new List<Parameter> ();
			ps.Add (p);
			var r = m.Post ("/items", ps, new {foo="bar"});

			Assert.AreEqual (HttpStatusCode.Created, r.StatusCode);
		}

		[Test]
		public void Put ()
		{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "valid token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;

			var ps = new List<Parameter> ();
			ps.Add (p);
			var r = m.Put ("/items/123", ps, new {foo="bar"});

			Assert.AreEqual (HttpStatusCode.OK, r.StatusCode);
		}

		[Test]
		public void PutWithRefreshToken ()
		{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "expired token", "valid refresh token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;

			var ps = new List<Parameter> ();
			ps.Add (p);
			var r = m.Put ("/items/123", ps, new {foo="bar"});

			Assert.AreEqual (HttpStatusCode.OK, r.StatusCode);
		}

		[Test]
		public void Delete ()
		{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "valid token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;

			var ps = new List<Parameter> ();
			ps.Add (p);
			var r = m.Delete ("/items/123", ps);

			Assert.AreEqual (HttpStatusCode.OK, r.StatusCode);
		}

		[Test]
		public void DeleteWithRefreshToken ()
		{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "expired token", "valid refresh token");

			var p = new Parameter ();
			p.Name = "access_token";
			p.Value = m.AccessToken;

			var ps = new List<Parameter> ();
			ps.Add (p);
			var r = m.Delete ("/items/123", ps);

			Assert.AreEqual (HttpStatusCode.OK, r.StatusCode);
		}

		[Test]
    	public void TestUserAgent() 
    	{
			Meli.ApiUrl = "http://localhost:3000";
			Meli m = new Meli (123456, "client secret", "expired token", "valid refresh token");

			var response = m.Get ("/echo/user_agent");
			Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);
	    }
	}
}