﻿using System.Collections.Generic;

namespace ADSD.Crypto
{
    /// <summary>
    /// Defines the inbound and outbound mapping for claim claim types from jwt to .net claim
    /// </summary>
    internal static class ClaimTypeMapping
    {
        private static readonly Dictionary<string, string> shortToLongClaimTypeMapping = new Dictionary<string, string>()
        {
            {
                "actort",
                "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor"
            },
            {
                "birthdate",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth"
            },
            {
                "email",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
            },
            {
                "family_name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"
            },
            {
                "gender",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender"
            },
            {
                "given_name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
            },
            {
                "nameid",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            },
            {
                "sub",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            },
            {
                "website",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/webpage"
            },
            {
                "unique_name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            },
            {
                "oid",
                "http://schemas.microsoft.com/identity/claims/objectidentifier"
            },
            {
                "scp",
                "http://schemas.microsoft.com/identity/claims/scope"
            },
            {
                "tid",
                "http://schemas.microsoft.com/identity/claims/tenantid"
            },
            {
                "acr",
                "http://schemas.microsoft.com/claims/authnclassreference"
            },
            {
                "adfs1email",
                "http://schemas.xmlsoap.org/claims/EmailAddress"
            },
            {
                "adfs1upn",
                "http://schemas.xmlsoap.org/claims/UPN"
            },
            {
                "amr",
                "http://schemas.microsoft.com/claims/authnmethodsreferences"
            },
            {
                "auth_time",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant"
            },
            {
                "authmethod",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod"
            },
            {
                "certapppolicy",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/applicationpolicy"
            },
            {
                "certauthoritykeyidentifier",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/authoritykeyidentifier"
            },
            {
                "certbasicconstraints",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/basicconstraints"
            },
            {
                "certeku",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/eku"
            },
            {
                "certissuer",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/issuer"
            },
            {
                "certissuername",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/issuername"
            },
            {
                "certkeyusage",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/keyusage"
            },
            {
                "certnotafter",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/notafter"
            },
            {
                "certnotbefore",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/notbefore"
            },
            {
                "certpolicy",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/certificatepolicy"
            },
            {
                "certpublickey",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/rsa"
            },
            {
                "certrawdata",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/rawdata"
            },
            {
                "certserialnumber",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/serialnumber"
            },
            {
                "certsignaturealgorithm",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/signaturealgorithm"
            },
            {
                "certsubject",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/subject"
            },
            {
                "certsubjectaltname",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/san"
            },
            {
                "certsubjectkeyidentifier",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/subjectkeyidentifier"
            },
            {
                "certsubjectname",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/subjectname"
            },
            {
                "certtemplateinformation",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/certificatetemplateinformation"
            },
            {
                "certtemplatename",
                "http://schemas.microsoft.com/2012/12/certificatecontext/extension/certificatetemplatename"
            },
            {
                "certthumbprint",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint"
            },
            {
                "certx509version",
                "http://schemas.microsoft.com/2012/12/certificatecontext/field/x509version"
            },
            {
                "clientapplication",
                "http://schemas.microsoft.com/2012/01/requestcontext/claims/x-ms-client-application"
            },
            {
                "clientip",
                "http://schemas.microsoft.com/2012/01/requestcontext/claims/x-ms-client-ip"
            },
            {
                "clientuseragent",
                "http://schemas.microsoft.com/2012/01/requestcontext/claims/x-ms-client-user-agent"
            },
            {
                "commonname",
                "http://schemas.xmlsoap.org/claims/CommonName"
            },
            {
                "denyonlyprimarygroupsid",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarygroupsid"
            },
            {
                "denyonlyprimarysid",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarysid"
            },
            {
                "denyonlysid",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/denyonlysid"
            },
            {
                "devicedispname",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/displayname"
            },
            {
                "deviceid",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/identifier"
            },
            {
                "deviceismanaged",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/ismanaged"
            },
            {
                "deviceostype",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/ostype"
            },
            {
                "deviceosver",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/osversion"
            },
            {
                "deviceowner",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/userowner"
            },
            {
                "deviceregid",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/registrationid"
            },
            {
                "endpointpath",
                "http://schemas.microsoft.com/2012/01/requestcontext/claims/x-ms-endpoint-absolute-path"
            },
            {
                "forwardedclientip",
                "http://schemas.microsoft.com/2012/01/requestcontext/claims/x-ms-forwarded-client-ip"
            },
            {
                "group",
                "http://schemas.xmlsoap.org/claims/Group"
            },
            {
                "groupsid",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid"
            },
            {
                "idp",
                "http://schemas.microsoft.com/identity/claims/identityprovider"
            },
            {
                "insidecorporatenetwork",
                "http://schemas.microsoft.com/ws/2012/01/insidecorporatenetwork"
            },
            {
                "isregistereduser",
                "http://schemas.microsoft.com/2012/01/devicecontext/claims/isregistereduser"
            },
            {
                "ppid",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier"
            },
            {
                "primarygroupsid",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid"
            },
            {
                "primarysid",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid"
            },
            {
                "proxy",
                "http://schemas.microsoft.com/2012/01/requestcontext/claims/x-ms-proxy"
            },
            {
                "pwdchgurl",
                "http://schemas.microsoft.com/ws/2012/01/passwordchangeurl"
            },
            {
                "pwdexpdays",
                "http://schemas.microsoft.com/ws/2012/01/passwordexpirationdays"
            },
            {
                "pwdexptime",
                "http://schemas.microsoft.com/ws/2012/01/passwordexpirationtime"
            },
            {
                "relyingpartytrustid",
                "http://schemas.microsoft.com/2012/01/requestcontext/claims/relyingpartytrustid"
            },
            {
                "role",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            },
            {
                "upn",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn"
            },
            {
                "winaccountname",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname"
            }
        };
        private static readonly IDictionary<string, string> longToShortClaimTypeMapping = (IDictionary<string, string>) new Dictionary<string, string>();
        private static readonly HashSet<string> inboundClaimFilter;

        static ClaimTypeMapping()
        {
            foreach (KeyValuePair<string, string> keyValuePair in shortToLongClaimTypeMapping)
            {
                if (longToShortClaimTypeMapping != null && !longToShortClaimTypeMapping.ContainsKey(keyValuePair.Value))
                    longToShortClaimTypeMapping.Add(keyValuePair.Value, keyValuePair.Key);
            }
            inboundClaimFilter = new HashSet<string>();
        }

        /// <summary>
        /// Gets the InboundClaimTypeMap used by JwtSecurityTokenHandler when producing claims from jwt.
        /// </summary>
        public static IDictionary<string, string> InboundClaimTypeMap
        {
            get
            {
                return shortToLongClaimTypeMapping;
            }
        }

        /// <summary>
        /// Gets the OutboundClaimTypeMap is used by JwtSecurityTokenHandler to shorten claim types when creating a jwt.
        /// </summary>
        public static IDictionary<string, string> OutboundClaimTypeMap
        {
            get
            {
                return longToShortClaimTypeMapping;
            }
        }

        public static ISet<string> InboundClaimFilter
        {
            get
            {
                return (ISet<string>) inboundClaimFilter;
            }
        }
    }
}