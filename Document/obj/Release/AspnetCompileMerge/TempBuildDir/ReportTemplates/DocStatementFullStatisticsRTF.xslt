<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="text"/>
  <xsl:template match="/">
    <xsl:text>{\rtf1\ansi\ansicpg1251\deff0\deflang1049\deflangfe1049\deftab708{\fonttbl{\f0\fswiss\fprq2\fcharset204 Calibri;}{\f1\froman\fprq2\fcharset204{\*\fname Times New Roman;}Times New Roman CYR;}}</xsl:text>

              
    <xsl:text>\viewkind4\uc1\pard\qc\lang1058\f0\fs24</xsl:text>
    <xsl:text>\'c2I\'c4\'ce\'cc\'ce\'d1\'d2I</xsl:text>
    <xsl:text>\par</xsl:text>
    <xsl:text>\'cf\'d0\'ce \'cd\'c0\'c4I\'d1\'cb\'c0\'cdI \'c7\'c0\'df\'c2\'c8 \'c2 \'ccI\'d1\'dc\'ca\'c2\'c8\'ca\'ce\'cd\'ca\'ce\'cc \'c7\'c0 \'cf\'c5\'d0I\'ce\'c4</xsl:text>
    <xsl:text>\par</xsl:text>
    <xsl:text>\pard\nowidctlpar\sa200\sl276\slmult1\qc\fs22</xsl:text>
    <xsl:text>\'c7 </xsl:text>
    <xsl:value-of select="DsFullStatistics/StartDate"/>
    <xsl:text> \'cf\'ce </xsl:text>
    <xsl:value-of select="DsFullStatistics/EndDate"/>
    <xsl:text>\fs28</xsl:text>
    <xsl:text>\par</xsl:text>
    <xsl:text>\trowd\trgaph108\trleft-108\trrh365\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>

    <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs</xsl:text>
    <xsl:text>\cellx818\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs</xsl:text>
    <xsl:text>\cellx8072\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs</xsl:text>
    <xsl:text>\cellx9575\pard\intbl\nowidctlpar\qc\fs24</xsl:text>
    <xsl:text>\cell</xsl:text>
    <xsl:text>\cell</xsl:text>
    <xsl:text>\'ca\'b3\'eb\'fc\'ea\'b3\'f1\'f2\'fc</xsl:text>
    <xsl:text>\cell</xsl:text>
    <xsl:text>\row</xsl:text>

    <xsl:for-each select="DsFullStatistics/Statistic/StatisticRow">
      
      <xsl:text>\trowd\trgaph108\trleft-108\trrh365\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>

      
      <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs</xsl:text>
      <xsl:if test="Count">
        <xsl:text>\clbrdrr\brdrw10\brdrs</xsl:text>
      </xsl:if>
      <xsl:text>\clbrdrb\brdrw10\brdrs \cellx818</xsl:text>

      <xsl:if test="Count">
        <xsl:text>\clbrdrl\brdrw10\brdrs</xsl:text>
      </xsl:if>
      <xsl:text>\clbrdrt\brdrw10\brdrs</xsl:text>
      <xsl:if test="Count">
        <xsl:text>\clbrdrr\brdrw10\brdrs</xsl:text>
      </xsl:if>
      <xsl:text>\clbrdrb\brdrw10\brdrs \cellx8072</xsl:text>

      <xsl:if test="Count">
        <xsl:text>\clbrdrl\brdrw10\brdrs</xsl:text>
      </xsl:if>
      <xsl:text>\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9575</xsl:text>
      
      <xsl:text>\pard\intbl\nowidctlpar </xsl:text>
      <xsl:value-of select="Number"/>
      <xsl:text>\cell </xsl:text>
      <xsl:value-of select="Name"/>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\pard\intbl\nowidctlpar\qr </xsl:text>
      <xsl:value-of select="Count"/>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\row</xsl:text>
        

      <xsl:text></xsl:text>
    </xsl:for-each>
    <xsl:text>\pard\sa200\sl276\slmult1\fs28</xsl:text>
    
    <xsl:text>\par</xsl:text>
    <xsl:text>\'c2\'e8\'ea\'ee\'ed\'e0\'e2 ______________________ \par</xsl:text>

    <xsl:text>}</xsl:text>

  </xsl:template>
</xsl:stylesheet>
