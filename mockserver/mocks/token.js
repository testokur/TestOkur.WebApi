"use strict";

module.exports = function(req, res) {
  if (req.body.client_id == "testokur.private.client") {
    res.json({
      access_token:
        "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE4MTJBRDkxNkMxNzk1OTZFQTFCMTQxRDk0OTE0QUMyRUIzMERFRkIiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJHQkt0a1d3WGxaYnFHeFFkbEpGS3d1c3czdnMifQ.eyJuYmYiOjE1NDY4ODM3NzQsImV4cCI6MTU0Njg4NzM3NCwiaXNzIjoiaHR0cHM6Ly9pZGVudGl0eS1xYS50ZXN0b2t1ci5jb20iLCJhdWQiOlsiaHR0cHM6Ly9pZGVudGl0eS1xYS50ZXN0b2t1ci5jb20vcmVzb3VyY2VzIiwidGVzdG9rdXJhcGkiXSwiY2xpZW50X2lkIjoidGVzdG9rdXIucHJpdmF0ZS5jbGllbnQiLCJzY29wZSI6WyJ0ZXN0b2t1cmFwaSJdfQ.b000tqdEvlfGRHLppOEcLT__LSfjfgAN_z2A83k0GxF7bjGV7xI6Hd4c9QFYj3LtW0yaQzyU2BC8ik27p8OLn5pzNGnkz_Kc8xpIbB8SJAs-Pcb-kg81fxxk3zXrSY4Bjj2WNBCP32RJSKLkkiOclVsyykJwx9OaeaLCSnMl2OIG7Ux2oq306cOCW43LK4aKAwlPZrAIOO9MsuTTdmENk48nHibkWfIDsKia5_BF9CsoJqwEDZYTQehHBBPLB73GGhSFm-5MHDipxyaxGIRSr5UXVyo13pahoeEXkkjxVJcDbQfiFtsUUSrkMyRgbmcIWmW0A57E8JLoFHmoCdJIdA",
      expires_in: "3600",
      token_type: "Bearer"
    });
  }
  if (req.body.client_id == "testokur.public.client") {
    res.json({
      access_token:
		"eyJhbGciOiJSUzI1NiIsImtpZCI6IjE4MTJBRDkxNkMxNzk1OTZFQTFCMTQxRDk0OTE0QUMyRUIzMERFRkIiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJHQkt0a1d3WGxaYnFHeFFkbEpGS3d1c3czdnMifQ.eyJuYmYiOjE1NDY5NzEwMTEsImV4cCI6MTU0Njk3NDYxMSwiaXNzIjoiaHR0cHM6Ly9pZGVudGl0eS1xYS50ZXN0b2t1ci5jb20iLCJhdWQiOlsiaHR0cHM6Ly9pZGVudGl0eS1xYS50ZXN0b2t1ci5jb20vcmVzb3VyY2VzIiwidGVzdG9rdXJhcGkiXSwiY2xpZW50X2lkIjoidGVzdG9rdXIucHVibGljLmNsaWVudCIsInNjb3BlIjpbInRlc3Rva3VyYXBpIl19.GY0RbbCyYpFIOGKVCBIlEn9_YRiAMgWwfsAAlFSas2_HGnyv3gRMVp_NOiUUjKWaZVweK61TBWGvCEl0H-poVy5N1CioaWDc32CZggDtAsoSstoZIBZnnochC9uhq-bWVjBXxWx-2B6zQFkMq34VCnKgzePQ7SzM73NHLhvf2f-9zFdpflTWJW12RyhcGnJGs83AyJHWsAeMDUlgi_9mRtdOKwOAPUFLTFoVfU036jADubPkOvvdZlU6MXpENGLoLHBXk0gOBE8D4HbehoWYXKgxsgo2qPxlzQiuBqiHmsbqqNBf0vyukno_ZJsCCBCvoEW-Z85Y-pXtohVwfBSHWg",
      expires_in: "3600",
      token_type: "Bearer"
    });
  }
};
