package com.microservices.employeespringservice;

import lombok.AllArgsConstructor;
import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.Setter;

/**
 * Employee
 */
@Getter
@Setter
@EqualsAndHashCode
@AllArgsConstructor
public class Employee {
    private String firstName;
    private String lastName;
    private String ipAddress;
}