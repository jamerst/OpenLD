import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'

export class FixtureLibrary extends Component {
  static displayName = FixtureLibrary.name;

  constructor(props) {
    super(props);
    this.state = { results: [], loading: true };
  }

  componentDidMount() {
    this.fetchFixtures("");
  }

  renderResults(results) {
    return (
       <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Name</th>
            <th>Manufacturer</th>
            <th>Power</th>
            <th>Weight</th>
          </tr>
        </thead>
        <tbody>
          {results.map(result =>
            <tr key={result.name}>
              <td>{result.name}</td>
              <td>{result.manufacturer}</td>
              <td>{result.power}</td>
              <td>{result.weight}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading results..</em></p>
      : this.renderResults(this.state.results);

    return (
      <div>
        <h1>Fixture Library</h1>
        {contents}
      </div>
    );
  }

  async fetchFixtures(search) {
    const token = authService.getAccessToken();
    const response = await fetch('api/library/GetFixtures', {
      method: "POST",
      headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
      body: JSON.stringify(search)
    });

    const data = await response.json();
    this.setState({results: data.data, loading: false});
  }
}
