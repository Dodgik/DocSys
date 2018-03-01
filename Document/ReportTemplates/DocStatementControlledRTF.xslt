<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="text"/>
    <xsl:template match="/">
      <xsl:text>{\rtf1\ansi\ansicpg1251\deff0\deflang1049\deflangfe1049\deftab708{\fonttbl{\f0\fswiss\fprq2\fcharset204 Calibri;}{\f1\froman\fprq2\fcharset204{\*\fname Times New Roman;}Times New Roman CYR;}}</xsl:text>
      
      <xsl:text>\viewkind4\uc1\pard\sa200\sl276\slmult1\qc\lang1058\f0\fs28</xsl:text>
      <xsl:text>\'d1\'ef\'e8\'f1\'ee\'ea \'e4\'ee\'ea\'f3\'ec\'e5\'ed\'f2\'b3\'e2, \'ff\'ea\'b3 \'f1\'f2\'ee\'ff\'f2\'fc \'ed\'e0 \'ea\'ee\'ed\'f2\'f0\'ee\'eb\'b3 \'e7\'e0 \'e2\'e8\'ea\'ee\'ed\'e0\'e2\'f6\'ff\'ec\'e8</xsl:text>
      <xsl:text>\par</xsl:text>
      <xsl:text>\trowd\trgaph108\trleft-108\trrh678\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
      
      <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
      <xsl:text>\cellx1210\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
      <xsl:text>\cellx3265\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
      <xsl:text>\cellx5060\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
      <xsl:text>\cellx6379\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
      <xsl:text>\cellx9781\pard\intbl\nowidctlpar </xsl:text>
      
      <xsl:text>\qc\fs24</xsl:text>
      <xsl:text>\'c4\'e0\'f2\'e0 \'e7\'e2\'e5\'f0\'ed\'e5\'ed\'ed\'ff</xsl:text>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\'cf\'f0\'b3\'e7\'e2\'e8\'f9\'e5 \'e7\'e0\'ff\'e2\'ed\'e8\'ea\'e0</xsl:text>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\'cd\'ee\'ec\'e5\'f0 \'e4\'ee\'ea\'f3\'ec\'e5\'ed\'f2\'e0</xsl:text>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\'c2\'e8\'ea\'ee\'ed\'e0\'f2\'e8 \'e4\'ee</xsl:text>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\'c7\'ec\'b3\'f1\'f2 \'e7\'e2\'e5\'f0\'ed\'e5\'ed\'ed\'ff</xsl:text>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\row</xsl:text>
      
      <xsl:for-each select="ArrayOfWorkerCards/WorkerCards">
        <xsl:text>\trowd\trgaph108\trleft-108\trrh412\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>

        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9781\pard\intbl\</xsl:text>
        <xsl:text>'c2\'e8\'ea\'ee\'ed\'e0\'e2\'e5\'f6\'fc: </xsl:text>
        <xsl:value-of select="Worker"/>
        <xsl:text>\cell</xsl:text>
        <xsl:text>\row</xsl:text>
        
        <xsl:for-each select="Cards/CardInfo">
          <xsl:text>\trowd\trgaph108\trleft-108\trrh404\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>

          <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
          <xsl:text>\cellx1210\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
          <xsl:text>\cellx3265\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
          <xsl:text>\cellx5060\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
          <xsl:text>\cellx6379\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs </xsl:text>
          <xsl:text>\cellx9781\pard\intbl\nowidctlpar </xsl:text>
          
          <xsl:value-of select="CreationDate"/>
          <xsl:text>\cell </xsl:text>
          <xsl:value-of select="Citizen"/>
          <xsl:text>\cell </xsl:text>
          <xsl:value-of select="Number"/>
          <xsl:text>\cell </xsl:text>
          <xsl:value-of select="EndDate"/>
          <xsl:text>\cell </xsl:text>
          <xsl:value-of select="Content"/>
          <xsl:text>\cell</xsl:text>
          <xsl:text>\row </xsl:text>

        </xsl:for-each>
        
        <xsl:text></xsl:text>
      </xsl:for-each>
      <xsl:text>\pard\sa200\sl276\slmult1\fs28</xsl:text>
            
      <xsl:text>\par</xsl:text>
      <xsl:text>\'c2\'e8\'ea\'ee\'ed\'e0\'e2 ______________________ \par</xsl:text>

      <xsl:text>}</xsl:text>

    </xsl:template>
</xsl:stylesheet>
