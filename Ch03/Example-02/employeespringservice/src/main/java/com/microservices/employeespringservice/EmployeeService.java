package com.microservices.employeespringservice;

import java.net.InetAddress;
import java.net.UnknownHostException;

import org.springframework.stereotype.Service;

/**
 * EmployeeService
 */
@Service
public class EmployeeService {

    public Employee GetEmployee(String firstName, String lastName){
        
        String ipAddress;
        
        try {
            
            ipAddress = InetAddress.getLocalHost().getHostAddress().toString();    

        } catch (UnknownHostException e) {

            ipAddress = e.getMessage();
        }

        Employee employee = new Employee(firstName, lastName,ipAddress);

        return employee;
    }
}