<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:o="urn:schemas-microsoft-com:office:office"
    xmlns:x="urn:schemas-microsoft-com:office:excel"
    xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
    <xsl:template match="/">
        <xsl:processing-instruction name="mso-application">
            <xsl:text>progid="Excel.Sheet"</xsl:text>
        </xsl:processing-instruction>
        <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
            xmlns:o="urn:schemas-microsoft-com:office:office"
            xmlns:x="urn:schemas-microsoft-com:office:excel"
            xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
            xmlns:html="http://www.w3.org/TR/REC-html40">
            <Styles>
                <Style ss:ID="Default" ss:Name="Normal">
                    <Alignment ss:Vertical="Bottom" />
                    <Borders />
                    <Font />
                    <Interior />
                    <NumberFormat />
                    <Protection />
                </Style>
                <Style ss:ID="s21">
                    <Font ss:Bold="1"/>
                    <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
                </Style>
            </Styles>
            <Worksheet ss:Name="Sheet 1">
                <Table>
                    <Row>
                        <Cell>
                            <Data ss:Type="String">
                                <xsl:value-of select="CardNumber" />
                            </Data>
                        </Cell>
                        <Cell>
                            <Data ss:Type="String">
                                <xsl:value-of select="StartDate" />
                            </Data>
                        </Cell>
                        <Cell>
                            <Data ss:Type="String">
                                <xsl:value-of select="EndDate" />
                            </Data>
                        </Cell>
                        <Cell>
                            <Data ss:Type="String">
                                <xsl:value-of select="ControlResponseDate" />
                            </Data>
                        </Cell>
                        <Cell>
                            <Data ss:Type="String">
                                <xsl:value-of select="ControlResponse" />
                            </Data>
                        </Cell>
                    </Row>
                </Table>
            </Worksheet>
        </Workbook>
    </xsl:template>
</xsl:stylesheet>