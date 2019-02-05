﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonsSamples.Lesson7.CohesionCoupling
{
    // One step towards better cohesion by grouping some of the fields in one, 
    //   and extract the file write functionality out

    public class PageXmlExport_3
    {
        private readonly IPageFileWriter fileWriter;

        private readonly int maxSalesOrders; // used in 3/5 methods
        private readonly bool addCustomerDetails; // used in 2/5 methods

        private readonly ICrmService crmService; // used in 2/5 methods
        private readonly ILocationService locationService; // used in 2/5 methods

        public PageXmlExport_3(
            IPageFileWriter fileWriter,
            int maxSalesOrders,
            bool addCustomerDetails,
            ICrmService crmService,
            ILocationService locationService)
        {
            this.fileWriter = fileWriter;
            this.maxSalesOrders = maxSalesOrders;
            this.addCustomerDetails = addCustomerDetails;
            this.crmService = crmService;
            this.locationService = locationService;
        }

        public bool ExportCustomerPage(string customerName)
        {
            PageXml content = new PageXml {Customer = new CustomerXml {Name = customerName}};

            using (EfRepository repository = new EfRepository())
            {
                if (maxSalesOrders > 0)
                {
                    var orders = repository.GetEntities<Order>()
                                           .Where(o => o.Customer.CompanyName == content.Customer.Name)
                                           .OrderBy(o => o.OrderDate)
                                           .Take(maxSalesOrders);

                    //enrich content with orders
                    // ...
                }

                if (addCustomerDetails)
                {
                    var customer = repository.GetEntities<Customer>()
                                             .Where(c => c.CompanyName == customerName);

                    // enrich content with customer data
                    // ...
                }
            }

            return fileWriter.WriteFile(content, "CustomerPage");
        }

        public bool ExportCustomerPageWithExternalData(
            string customerName,
            PageData externalData)

        {
            PageXml content = new PageXml {Customer = new CustomerXml {Name = customerName}};

            if (externalData.CustomerData != null)
            {
                // enrich with externalData.CustomerData 
                // ...
            }
            else
            {
                CustomerInfo customerData = crmService.GetCustomerInfo(content.Customer.Name);
            }

            using (EfRepository repository = new EfRepository())
            {
                if (maxSalesOrders > 0)
                {
                    var orders = repository.GetEntities<Order>()
                                           .Where(o => o.Customer.CompanyName == content.Customer.Name)
                                           .OrderBy(o => o.OrderDate)
                                           .Take(maxSalesOrders);

                    //enrich content with orders
                    // ...
                }

                if (addCustomerDetails)
                {
                    var customer = repository.GetEntities<Customer>()
                                             .Where(c => c.CompanyName == customerName);

                    // enrich content by merging the external customer data with what read from DB
                    // ...
                }
            }

            if (locationService != null)
            {
                foreach (var address in content.Customer.Addresses)
                {
                    Coordinates coordinates = locationService.GetCoordinates(address.City, address.Street, address.Number);
                    if (coordinates != null)
                        address.Coordinates = string.Format("{0},{1}", coordinates.Latitude, coordinates.Longitude);
                }
            }

            return fileWriter.WriteFile(content);
        }

        public bool ExportOrders(string customerName)
        {
            PageXml content = new PageXml {Customer = new CustomerXml {Name = customerName}};

            using (EfRepository repository = new EfRepository())
            {
                var orders = repository.GetEntities<Order>()
                                       .Where(o => o.Customer.CompanyName == content.Customer.Name)
                                       .OrderBy(o => o.ApprovedAmmount)
                                       .ThenBy(o => o.OrderDate);

                //enrich content with orders
            }

            return fileWriter.WriteFile(content);
        }

        public IEnumerable<PageXml> GetPagesFromOrders(IEnumerable<Order> orders)

        {
            Dictionary<string, IEnumerable<Order>> customerOrders = GroupOrdersByCustomer(orders);
            foreach (var customerName in customerOrders.Keys)
            {
                PageXml content = new PageXml {Customer = new CustomerXml {Name = customerName}};

                if (crmService != null)
                {
                    CustomerInfo customerData = crmService.GetCustomerInfo(content.Customer.Name);
                    //enrich with data from crm
                }

                var recentOrders = customerOrders[customerName]
                    .OrderBy(o => o.OrderDate)
                    .Take(maxSalesOrders);
                foreach (var order in recentOrders)
                {
                    // enrich content with orders
                    // ...
                }

                if (locationService != null)
                {
                    foreach (var address in content.Customer.Addresses)
                    {
                        Coordinates coordinates = locationService.GetCoordinates(address.City, address.Street, address.Number);
                        if (coordinates != null)
                            address.Coordinates = string.Format("{0},{1}", coordinates.Latitude, coordinates.Longitude);
                    }
                }

                yield return content;
            }
        }

        public bool ExportPagesFromOrders(IEnumerable<Order> orders)
        {
            IEnumerable<PageXml> pages = GetPagesFromOrders(orders);
            foreach (var pageXml in pages)
            {
                bool wasWritten = fileWriter.WriteFile(pageXml, "OrdersPage");
                if (!wasWritten)
                    return false;
            }
            return true;
        }

        private Dictionary<string, IEnumerable<Order>> GroupOrdersByCustomer(IEnumerable<Order> orders)
        {
            // group orders by customer name and return them in a dictionary, ordered by OrderDate
            throw new NotImplementedException();
        }
    }
}