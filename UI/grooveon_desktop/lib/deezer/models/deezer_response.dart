import 'package:json_annotation/json_annotation.dart';

part 'deezer_response.g.dart';

@JsonSerializable(genericArgumentFactories: true)
class DeezerResponse<T> {
  final List<T> data;
  final int? total;
  final String? next;

  DeezerResponse({
    required this.data,
    this.total,
    this.next,
  });

  factory DeezerResponse.fromJson(
    Map<String, dynamic> json,
    T Function(Object? json) fromJsonT,
  ) =>
      _$DeezerResponseFromJson(json, fromJsonT);

  Map<String, dynamic> toJson(Object? Function(T value) toJsonT) =>
      _$DeezerResponseToJson(this, toJsonT);
}