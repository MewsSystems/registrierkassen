﻿using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mews.Registrierkassen.Dto;
using Microsoft.IdentityModel.Tokens;

namespace Mews.Registrierkassen
{
    public class OfflineSigner
    {
        public OfflineSigner(X509Certificate2 certificate)
        {
            Certificate = certificate;
        }

        public X509Certificate2 Certificate { get; }

        public SignerOutput Sign(QrData qrData)
        {
            //// This is a manual JWS implementation as RKSV does not use standard signature format. Do not migrate to jose-jwt
            var jwsHeaderBase64Url = "eyJhbGciOiJFUzI1NiJ9"; // Fixed value for RKSV
            var jwsPayloadBase64Url = Base64UrlEncoder.Encode(qrData.Value);
            var jwsDataToBeSigned = jwsHeaderBase64Url + "." + jwsPayloadBase64Url;

            var bytes = Certificate.GetECDsaPrivateKey().SignData(Encoding.UTF8.GetBytes(jwsDataToBeSigned), HashAlgorithmName.SHA256);
            var jwsSignatureBase64Url = Base64UrlEncoder.Encode(bytes);

            var signerOutput = jwsHeaderBase64Url + "." + jwsPayloadBase64Url + "." + jwsSignatureBase64Url;
            return new SignerOutput(new SignatureResponse { JwsRepresentation = signerOutput }, qrData);
        }
    }
}