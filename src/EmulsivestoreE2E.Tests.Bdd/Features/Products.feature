Feature: Emulsive Store Products Page
    As a photographer
    I want to be able to sort sort results on the products page
    So that I can easily locate items

Background:
    Given am on the Products page

Scenario: Verify Reset reverts all filter options
    And Filter on the following options:
    | FilterOption | Value              |
    | Keyword      | Portra             |
    | Format       | 35mm               |
    | Manufacturer | Kodak              |
    | OrderBy      | highest-price-desc |
    | OnSale       | checked            |
    | Price        | 2000               |
    When I click on Reset
    Then the filters will revert back to default

#Scenario: Verify Keyword Filter
#    When I sort on the Keyword with value 'Portra'
#    Then the results on the first page will contain the keyword in either title, description or manufacturer
#    And the Format option will contain:
#      |120mm |
#      |35mm  |