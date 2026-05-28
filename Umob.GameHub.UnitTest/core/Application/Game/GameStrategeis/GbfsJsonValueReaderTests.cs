using NUnit.Framework;
using Umob.GameHub.Application.Game.GameStrategeis;

namespace Umob.GameHub.Application.UnitTests.Game.GameStrategies;

[TestFixture]
public sealed class GbfsJsonValueReaderTests
{
	[Test]
	public void ReadFirstValue_WhenCollectionPathAndFieldExist_ShouldReturnFirstValue()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "num_vehicles_available": 18
                    },
                    {
                        "station_id": "2",
                        "num_vehicles_available": 25
                    }
                ]
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadFirstValue(
			json,
			"data.stations",
			"num_vehicles_available");

		// Assert
		Assert.That(result, Is.EqualTo("18"));
	}

	[Test]
	public void ReadFirstValue_WhenFieldIsString_ShouldReturnStringValue()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "name": "Dubai Center"
                    }
                ]
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadFirstValue(
			json,
			"data.stations",
			"name");

		// Assert
		Assert.That(result, Is.EqualTo("Dubai Center"));
	}

	[Test]
	public void ReadFirstValue_WhenCollectionPathDoesNotExist_ShouldReturnNull()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": [
                    {
                        "num_vehicles_available": 18
                    }
                ]
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadFirstValue(
			json,
			"data.invalid",
			"num_vehicles_available");

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void ReadFirstValue_WhenFieldDoesNotExist_ShouldReturnNull()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": [
                    {
                        "num_vehicles_available": 18
                    }
                ]
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadFirstValue(
			json,
			"data.stations",
			"wrong_field");

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void ReadFirstValue_WhenCollectionIsEmptyArray_ShouldReturnNull()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": []
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadFirstValue(
			json,
			"data.stations",
			"num_vehicles_available");

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void ReadNumericValues_WhenCollectionContainsNumbers_ShouldReturnAllNumericValues()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "num_vehicles_available": 18
                    },
                    {
                        "station_id": "2",
                        "num_vehicles_available": 25
                    },
                    {
                        "station_id": "3",
                        "num_vehicles_available": 7
                    }
                ]
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadNumericValues(
			json,
			"data.stations",
			"num_vehicles_available");

		// Assert
		Assert.That(result, Is.EqualTo(new List<decimal> { 18, 25, 7 }));
	}

	[Test]
	public void ReadNumericValues_WhenSomeItemsDoNotHaveField_ShouldIgnoreThem()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "num_vehicles_available": 18
                    },
                    {
                        "station_id": "2"
                    },
                    {
                        "station_id": "3",
                        "num_vehicles_available": 7
                    }
                ]
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadNumericValues(
			json,
			"data.stations",
			"num_vehicles_available");

		// Assert
		Assert.That(result, Is.EqualTo(new List<decimal> { 18, 7 }));
	}

	[Test]
	public void ReadNumericValues_WhenCollectionPathDoesNotExist_ShouldReturnEmptyList()
	{
		// Arrange
		var json = """
        {
            "data": {
                "stations": [
                    {
                        "num_vehicles_available": 18
                    }
                ]
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadNumericValues(
			json,
			"data.invalid",
			"num_vehicles_available");

		// Assert
		Assert.That(result, Is.Empty);
	}

	[Test]
	public void ReadNumericValues_WhenPathPointsToSingleObject_ShouldReturnSingleValue()
	{
		// Arrange
		var json = """
        {
            "data": {
                "station": {
                    "station_id": "1",
                    "num_vehicles_available": 18
                }
            }
        }
        """;

		// Act
		var result = GbfsJsonValueReader.ReadNumericValues(
			json,
			"data.station",
			"num_vehicles_available");

		// Assert
		Assert.That(result, Is.EqualTo(new List<decimal> { 18 }));
	}
}