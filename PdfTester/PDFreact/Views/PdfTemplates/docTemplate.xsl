<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
<xsl:output method="xml" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"  doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd" encoding="us-ascii" />
 
  
<xsl:decimal-format name="european" decimal-separator="," grouping-separator="."/>
<xsl:decimal-format name="american" decimal-separator="." grouping-separator=","/>
<xsl:decimal-format name="international" decimal-separator="." grouping-separator=","/>

  <xsl:template match="Document">
    <xsl:variable name="orderSubTotal">
      <xsl:call-template name="sumLines">
        <xsl:with-param name="body" select="Content/Body"/>
        <xsl:with-param name="index" select="'1'"/>
        <xsl:with-param name="runningTotal" select="'0'"/>
      </xsl:call-template>
    </xsl:variable> 

<html>
  <head>
    <title>Saph Invoice</title>
  </head>
  <body class="main">
    <div class="companyinformation">
      <xsl:for-each select="Content/Header/Element">
        <xsl:value-of select="@Attribute"/>: <xsl:value-of select="."/><br />
      </xsl:for-each>
    </div>

    <hr class="invisible" />
    
     <table class="postable">
     <thead>
          <tr class="postheader">
           <td class="right">PhoneNumber</td>
           <td class="right">Minutes</td>
           <td class="right">Cost</td>
          </tr>
      </thead>
      <tbody>
    <xsl:call-template name="body">
          <xsl:with-param name="body" select="Content/Body"/>
          <xsl:with-param name="index" select="1"/>
          <xsl:with-param name="runningTotal" select="0"/>
      </xsl:call-template>
 <tr class="postfooter">
       <td></td>
       <td></td>
       <td></td>
       <td class="right">Subtotal</td>
       <td class="right">EUR</td>
       <td class="subtotal"> 
          <xsl:value-of select="format-number($orderSubTotal, '#,###.00', 'international')"/>
       </td>
       <td style="text-align:right"></td>
      </tr>
      
      <tr class="postfooter">
       <td></td>
       <td></td>
       <td></td>
       <td class="right">Total</td>
       <td class="right">EUR</td>
       <td class="subtotal">
          <xsl:value-of select="format-number($orderSubTotal, '#,###.00', 'international')"/>
      </td>
      <td></td>
      </tr>
      </tbody>
     </table>

     <!--<div class="posttext">
       <p>The invoice amount must be paid with indication of the "Voucher no." by remittance to our bank account indicated below free of bank charges for us. You may also send us a collection-only check in the currency mentioned above.</p>
       <p>Account Holder: <xsl:value-of select="vendor/companyname"/><br />
       Account Number: <xsl:value-of select="vendor/bankaccno"/><br />
       IBAN: <xsl:value-of select="vendor/iban"/><br />
       Bank Name: <xsl:value-of select="vendor/bankname"/><br />
       Bank Address: <xsl:value-of select="vendor/bankaddress"/><br />
       Bank SWIFT/BIC Code: <xsl:value-of select="vendor/swiftbic"/><br />
       Bank Code: <xsl:value-of select="vendor/bankcode"/></p>

       <barcode style="float: right; width: 2cm;">
          <xsl:attribute name="message">
            <xsl:value-of select="voucher/@processno"/>;<xsl:value-of select="voucher/@no"/>;<xsl:value-of select="voucher/@date"/>;<xsl:value-of select="voucher/@clientrefid"/>;<xsl:value-of select="format-number(($orderSubTotal * 1.19), '#.00', 'international')"/>;<xsl:value-of select="orderer/companyname"/>;<xsl:value-of select="orderer/personname"/>
          </xsl:attribute>
            <datamatrix/>
       </barcode>
       <p class="terms">Terms of payment:</p>

       <p>10 Days without deduction
          <xsl:value-of select="format-number(($orderSubTotal * 1.19), '#,###.00', 'international')"/>  EUR
       </p>
     </div>-->
   </body>
 </html>  
</xsl:template>
  
  <xsl:template name="sumLines">
    <xsl:param name="body"/>
    <xsl:param name="index" select="'1'"/>
    <xsl:param name="runningTotal" select="'0'"/>
    <xsl:variable name="currentline">
      <xsl:value-of select="$body/Line[$index]/Element[@Attribute='Cost']"/>
    </xsl:variable>
    <xsl:variable name="remaininglines">
      <xsl:choose>
        <xsl:when test="$index=count($body/Line)">
          <xsl:text>0</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:call-template name="sumLines">
            <xsl:with-param name="body" select="$body"/>
            <xsl:with-param name="index" select="$index+1"/>
            <xsl:with-param name="runningTotal" select="$runningTotal+$currentline"/>
          </xsl:call-template>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:value-of select="$currentline+$remaininglines"/>
  </xsl:template>
  
  <xsl:template name="body">
    <xsl:param name="body" />
    <xsl:param name="index" select="'1'"/>
    <xsl:param name="runningTotal" select="'0'"/>
    
    <xsl:variable name="line" select="$body/Line[$index]" />
    <xsl:variable name="cost" select="$body/Line[$index]/Element[@Attribute='Cost']" />
    <xsl:variable name="runningTotalSum" select="$runningTotal+$cost" />
     
    <xsl:call-template name="line">
      <xsl:with-param name="line" select="$line"/>
      <xsl:with-param name="index" select="$index"/>
      <xsl:with-param name="cost" select="$cost"/>
      <xsl:with-param name="runningTotal" select="$runningTotalSum"/>
    </xsl:call-template>
    
    <xsl:if test="$index &lt; count($body/Line)">
        <xsl:call-template name="body">
            <xsl:with-param name="body" select="$body" />
            <xsl:with-param name="index" select="$index+1" />
            <xsl:with-param name="runningTotal" select="$runningTotalSum" />
        </xsl:call-template>
    </xsl:if>
  </xsl:template>
  
  <xsl:template name="line">
    <xsl:param name="line"/>
    <xsl:param name="index"/>
     <xsl:param name="cost"/>
    <xsl:param name="runningTotal" select="'0'"/>   
    <tr>
        <xsl:for-each select="$line/Element">
          <td>
          <xsl:value-of select="."/>
          </td>
      </xsl:for-each>
    </tr>
    <td class="right"><div class="sum-subtotal"><xsl:value-of select="format-number($runningTotal, '#,###.00', 'international')"/></div></td>
  
  </xsl:template>
  
 </xsl:stylesheet>
