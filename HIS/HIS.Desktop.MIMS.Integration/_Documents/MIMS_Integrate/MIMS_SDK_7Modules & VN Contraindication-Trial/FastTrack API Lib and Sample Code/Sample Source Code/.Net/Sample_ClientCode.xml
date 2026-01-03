
import java.util.Base64;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

import com.sun.jersey.api.client.Client;
import com.sun.jersey.api.client.ClientResponse;
import com.sun.jersey.api.client.WebResource;
import com.sun.jersey.api.client.config.ClientConfig;
import com.sun.jersey.api.client.config.DefaultClientConfig;
import com.sun.jersey.api.representation.Form;

public class SampleConsumeCode {

    public static void main(String[] args) throws InterruptedException {
        
        String endpoint1 = "http://49.249.193.198:8081/RESTFTWebService-VN/FTRequest/xmlrequestplus"; // MIMS CDS
        String endpoint2 = "http://49.249.193.198:8081/RESTFTWebService-VN/FTRequest/xmlrequestplus-vn"; //VN_Contraindication
     
        
     // Your prescription query for endpoint1(MIMS CDS Alerts)
        String prescriptionquery1 = "<Request>\r\n"
        		+ "  <Interaction>\r\n"
        		+ "    <Prescribing>\r\n"
        		+ "       <Product reference=\"{E34D6E1E-3754-486D-829C-1AEE542FBC6F}\" />\r\n"
        		+ "       <Product reference=\"{C5F23AAC-1A4A-4094-9D29-46B585337DEE}\" />\r\n"
        		+ "    </Prescribing>\r\n"
        		+ "    <DuplicateTherapy checkSameDrug=\"true\" />\r\n"
        		+ "    <DuplicateIngredient checkSameDrug=\"true\" />\r\n"
        		+ "    <References/>\r\n"
        		+ "  </Interaction>\r\n"
        		+ "  <PatientProfile>\r\n"
        		+ "    <Gender>M</Gender>\r\n"
        		+ "    <Age>\r\n"
        		+ "      <Year>74</Year>\r\n"
        		+ "    </Age>\r\n"
        		+ "  </PatientProfile>\r\n"
        		+ "</Request>"; 
        
     // Your prescription query for endpoint2(VN_Contraindication Alerts)
        String prescriptionquery2 = "<Request>\r\n"
        		+ "<Interaction>\r\n"
        		+ "<Prescribing>\r\n"
        		+ "<ItemCode=\"12472\" />\r\n"
        		+ "<ItemCode=\"12899\" />\r\n"
        		+ "</Prescribing>\r\n"
        		+ "</Interaction>\r\n"
        		+ "</Request>"; 

        String responseXML1 = consumeEndpoint(endpoint1, prescriptionquery1);
        String responseXML2 = consumeEndpoint(endpoint2, prescriptionquery2);

        System.out.println("Response for MIMS CDS Alerts from Endpoint 1:\n" + responseXML1);
        System.out.println("---------------------------------------\n\n");
        System.out.println("---------------------------------------\n");
        System.out.println("Response for VN_Contraindication  Alerts from Endpoint 2:\n" + responseXML2);
    }
    
 


    private static String consumeEndpoint(String endpoint, String prescriptionQuery) {
        Map<String, String> formDataMap = new HashMap<>();
        formDataMap.put("prescriptionquery", prescriptionQuery);//// "prescriptionquery" is a mandatory paramerter to pass all valid request.xml api calls.
        formDataMap.put("responsetype", "html");// "responsetype" is a mandatory parameter which allows you to choose the output as "xml" or "html"
     // "alertfilterbydrug" is an optional paramerter to filter alerts by specific drug (sigle or multiple drugs), hence accordinly include the GUID's of all those drugs whose alerts need to be displayed in the final alert response.Hence uncomment below 4 lines to use this paramerter
    	
        StringBuilder alertfilterbydrug = new StringBuilder(); // For GUID's
    	  alertfilterbydrug.append("<GUIDS>"); //
    	 alertfilterbydrug.append("<GUID>{E34D6E1E-3754-486D-829C-1AEE542FBC6F}</GUID>");
    	 alertfilterbydrug.append("<GUID>{C5F23AAC-1A4A-4094-9D29-46B585337DEE}</GUID>");
    	 alertfilterbydrug.append("<GUID>{02EA3A43-8EC0-454B-B307-614A392C7475}</GUID>");
    	 alertfilterbydrug.append("<GUID>{A4DAF210-15C5-4CEF-9306-C5B9A12C455D}</GUID>");
    	 alertfilterbydrug.append("<GUID>{02EA3A43-8EC0-454B-B307-614A392C7475}</GUID>");
        alertfilterbydrug.append("</GUIDS>");


    	// "alertfilterbyseverity" is an optional parameter to filter alerts by severity, hence include all severties to display alerts only with these severities while alerts with other severities will be removed or not shown. Hence uncomment below 7 lines to use this paramerter.
    			
    	StringBuilder alertfilterbyseverity = new StringBuilder();

    	 alertfilterbyseverity.append("<WARNINGIDS>");
    	 alertfilterbyseverity.append("<WARNINGID>DP:X</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DP:A</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DP:B</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DP:C</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DP:D</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DL:Avoid if possible</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DL:Contraindicated</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DI:1</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DI:2</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DT:2</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>DT:3</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>D2H:Contraindicated</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>D2D:Severe</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>D2D:Minor</WARNINGID>");
    	 alertfilterbyseverity.append("<WARNINGID>D2D:Caution</WARNINGID>");
    	 alertfilterbyseverity.append("</WARNINGIDS>");
     // Both "alertfilterbydrug" and "alertfilterbyseverity" are optional paramerters, hence use or don't include this paramerter
		
     		 formDataMap.put("alertfilterbydrug", alertfilterbydrug.toString());				   
     		 formDataMap.put("alertfilterbyseverity",alertfilterbyseverity.toString()); 
     		//formDataMap.put("alertfilterbytemplate","yes"); //By Default alertfilterbytemplate Value is"Yes" AND alertfilterbytemplate' and  'alertfilterbydrug' parameters cannot be used together, instead use anyone of the parameter
     			
     		
        Form form = new Form();
        Iterator<String> itr = formDataMap.keySet().iterator();
        while (itr.hasNext()) {
            String key = itr.next();
            form.add(key, formDataMap.get(key));
        }

        String name = "MIMS";
        String password = "45shyt87fG";
        String authString = name + ":" + password;
        String authStringEnc = new String(Base64.getEncoder().encode(authString.getBytes()));

        Client restClient = Client.create();
        WebResource webResource = restClient.resource(endpoint);
        ClientResponse resp = webResource
                .header("Authorization", "Basic " + authStringEnc)
                .header("Content-Type", "application/x-www-form-urlencoded")
                .post(ClientResponse.class, form);

        if (resp.getStatus() != 200) {
            System.err.println("Unable to connect to the server");
            return null; // Or throw an exception
        }

        return resp.getEntity(String.class);
    }
}
