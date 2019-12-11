import React from 'react';

export class FixtureResults {
  static renderResults(results) {
    if (results.length > 0) {
      return (
         <table className='table table-striped' aria-labelledby="tabelLabel">
          <thead>
            <tr>
              <th>Name</th>
              <th>Manufacturer</th>
              <th>Type</th>
              <th>Power</th>
              <th>Weight</th>
            </tr>
          </thead>
          <tbody>
            {results.map(result =>
              <tr key={result.name}>
                <td>{result.name}</td>
                <td>{result.manufacturer}</td>
                <td>{result.type.name}</td>
                <td>{result.power}W</td>
                <td>{result.weight}kg</td>
              </tr>
            )}
          </tbody>
        </table>
      );
    } else {
      return (
        <p className="text-center"><em>No results found</em></p>
      )
    }
  }
}