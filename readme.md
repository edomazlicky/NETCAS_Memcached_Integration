Memcached Integration for the Jasig DotNet CAS Client

This is a C# library that provides Memcached integration services for the Jasig DotNet CAS Client. If you
merge the following github branch into your DotNetCasClient you can utilize this library:
https://github.com/edomazlicky/dotnet-cas-client/tree/NETC-51
(pull request is in progress)

Then you would change the CAS Client config attributes proxyTicketManager and ServiceTicketManager to this:

proxyTicketManager="Cas.Integration.Memcached.MemcachedProxyTicketManager, CAS_Memcached_Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" 

serviceTicketManager="Cas.Integration.Memcached.MemcachedServiceTicketManager, CAS_Memcached_Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" 


In your .NET app that utlizies the CAS Client you would need to add an Enyim Caching configuration section as well like this:
In configSections:
```XML
<configSections>
    <section name="casClientConfig" type="DotNetCasClient.Configuration.CasClientConfiguration, DotNetCasClient"/>
<!-- add Enyim to Config Sections -->
 <sectionGroup name="enyim.com">
      <section name="memcached" type="Enyim.Caching.Configuration.MemcachedClientSection,Enyim.Caching"/>
    </sectionGroup>
</configSections>

  <enyim.com>
    <memcached protocol="Binary">
      <servers>
        <add address="your.memcached.server" port="11211"/>        
      </servers>
    </memcached>
  </enyim.com>
```

 
