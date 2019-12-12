import React from 'react';
import { Card, CardImg, CardText, CardBody, CardColumns, CardDeck, CardTitle } from 'reactstrap';

export class FixtureResults {
  static renderResults(results, clickHandler) {
    if (results.length > 0) {
      return (
        <CardColumns>
          {results.map(result =>
            <Card color="light" onClick={() => clickHandler(result.id)}>
              <CardImg src={ "/api/fixture/GetImage/" + result.id }></CardImg>
              <CardBody>
                <CardTitle className="h3">{result.name}</CardTitle>
                <CardText>
                  <dl>
                    <dd>{result.manufacturer}</dd>
                    <dd>{result.type.name}</dd>

                    <dt>Power</dt>
                    <dd>{result.power}W</dd>
                  </dl>
                </CardText>
              </CardBody>
            </Card>
          )}
        </CardColumns>
      );
    } else {
      return (
        <p className="text-center"><em>No results found</em></p>
      )
    }
  }
}