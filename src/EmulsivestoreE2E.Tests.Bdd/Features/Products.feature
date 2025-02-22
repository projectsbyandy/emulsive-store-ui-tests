Feature: Emulsive Store Products Page
    As a photographer
    I want to be able to sort sort results on the products page
    So that I can easily locate items

Background:
    Given am on the Products page

Scenario: Verify updating the filter options are persistetd
    When Filtering on the following options:
      | FilterOption | Value              |
      | Keyword      | Kodak              |
      | Format       | 35mm               |
      | Manufacturer | Kodak              |
      | OrderBy      | highest-price-desc |
      | OnSale       | checked            |
      | Price        | 2500               |
    Then the values will be persisted in the filter options

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

# Full filtering tests performed in the API layer
Scenario: Verify Filtering on Keyword return associated results on 1st Page
    When Filtering on the following options:
       | FilterOption | Value  |
       | Keyword      | Kodak  |
    Then products in the 1st page will contain the keyword in either the title, description or manufacturer